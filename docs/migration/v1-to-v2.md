# Skywalker v1.x → v2.0 迁移指南

> 状态：**RC 候选迁移手册**　|　目标版本：**v2.0.0**　|　最后更新：2026-06-06

本文档是 v1.x 用户升级到 v2.0 的**权威迁移手册**。每一项 breaking change 在 `release/2.0` 落地时，**同一个 PR 必须同步更新本文档的对应条目**，否则不合并。

---

## 📚 如何阅读本文档

- 按模块组织，每个 breaking change 独立成一个小节。
- 每个小节遵循统一结构：**现象 → 原因 → 迁移步骤**。
- 未标注"已落地"的条目是**规划中**，API 细节可能微调。
- 配套：[v2.0 路线图](../architecture/v2.0-roadmap.md) | [SG 规范](../architecture/source-generators-spec.md)

---

## 🧭 升级总览

### 版本矩阵

| 包家族 | v1.x 最后版本 | v2.0 首个 GA | 兼容性 |
|---|---|---|---|
| `Skywalker.Ddd.*` | 1.x (LTS 6 个月) | 2.0.0 | **Breaking** |
| `Skywalker.Extensions.DynamicProxies*` | 1.x | 2.0.0 | **Breaking**（移除 Castle.Core） |
| ~~`Skywalker.Messaging.*` / `Skywalker.Transport.*`~~ | 1.0.1（最终版，带 `[TypeForwardedTo]`）| —（已迁出）| **迁移到独立项目 Vertex (polyrepo)** —— 改用 `Vertex.*` NuGet 包 ([`vertex-dotnet`](https://github.com/dengxuan/vertex-dotnet))，或 Go 的 `github.com/dengxuan/vertex-go`；详见 §5 |
| `Skywalker.EventBus.*` | 1.x | 2.0.0 | 基本兼容（Handler 自动发现改为 SG） |
| `Skywalker.Caching.*` / `Skywalker.Localization.*` / `Skywalker.Permissions.*` / `Skywalker.Settings.*` | 1.x | 2.0.0 | 基本兼容 |

### 升级路径

1. **试 preview 包**：`2.0.0-preview.N`（来自 `release/2.0` 分支自动发布）
2. **对照本文档**逐模块迁移
3. 给声明 source-generator 覆盖类型的项目添加对应 analyzer 包引用；使用 Skywalker NuGet 包时 analyzer 随 generator 包进入 `analyzers/dotnet/cs`。
4. 跑测试，确认无 source-generator 诊断和 AOT/trim 警告（参考 [`7. NativeAOT 零警告验证`](#7-nativeaot--零警告验证)）。
5. 发现遗漏请提 issue 并打标签 `migration`

---

## 1. DI 服务注册

### 1.1 `AddSkywalker()` 统一入口

**状态**：✅ 已落地（preview.5 scope，#254、#278-#284）

**现象**：v1.x 需要多个 `AddXxx()` 调用散落各处，启动代码冗长。

**v2.0 行为**：

- `AddSkywalker()` 会先消费 generated DI registrar metadata，再保留现有 FeatureProvider fallback。
- `[Service]`、`[ApplicationService]`、`[Repository]`、`[EventHandler]` 可触发 DI auto-registration source generator。
- 支持 interface fallback、显式 `ServiceType`、Scoped/Transient lifetime 映射。
- `ServiceType` 不能赋值给 implementation 时报告 `SKY1002`。

推荐的 v2.0 启动形状是保留一处统一入口，再由各模块扩展配置数据库、缓存、事件总线等能力：

```csharp
// v1.x
builder.Services.AddSkywalker()
    .AddAspNetCore();
builder.Services.AddSkywalkerDbContext<AppDbContext>(...);
builder.Services.AddEventBusLocal();
builder.Services.AddRedisCaching(...);

// v2.0
builder.Services.AddSkywalker();
builder.Services.AddSkywalkerDbContext<AppDbContext>(options =>
{
    options.Configure(context => context.DbContextOptions.UseMySql(...));
});
```

**迁移步骤**：

1. 把可自动注册的应用服务、仓储、事件处理器标注为 `[ApplicationService]`、`[Repository]`、`[EventHandler]` 或通用 `[Service]`。
2. 如果默认 interface fallback 不是想要的服务类型，使用 `ServiceType = typeof(IMyService)` 显式指定。
3. 确认 implementation 实现了显式 `ServiceType`；否则按 [SKY1002](../diagnostics/SKY1002.md) 修复。
4. 保留 `AddSkywalker()` 作为统一入口；已有手动注册仍可在 `AddSkywalker()` 前后按 DI ordering 规则覆盖。

**已知限制**：preview.5 不支持 open generic implementation registration、runtime-only dynamic service discovery、private nested implementation types 或运行时 factory 生成。详见 [DI auto-registration SG contract](../architecture/di-auto-registration-sg-contract.md)。

---

## 2. Repository 与 EF Core 集成

### 2.1 仓储注册反射 → Source Generator

**状态**：✅ 已落地（Sprint 1 generated-registration scope，#239-#242）

**现象**：v1.x 在启动期用 `MakeGenericType` + 反射为每个实体注册 `IRepository<T, TKey>` 和 `IDbSet<T>` 相关服务。

**v2.0 行为**：
- 在 `DbContext` 内由 SG 扫描 `DbSet<T>` 属性，编译期生成 repository/domain-service registration metadata。
- `AddSkywalkerDbContext<TDbContext>()` 优先调用 generated registration。
- Reflection fallback 仅作为 non-AOT 兼容路径保留；NativeAOT/dynamic-code-disabled 场景应依赖 generated metadata。

**迁移步骤**：
1. 在消费项目中引用 EF Repository source generator analyzer。
2. 保持 `DbSet<TEntity>` 为 public instance property，并确保 entity 类型可被 generated code 访问。
3. 确保实体实现 `IEntity`，且不是 abstract class。
4. 遇到 `SKY3001`-`SKY3006` 时按对应诊断页修复。
5. 继续通过 `AddSkywalkerDbContext<TDbContext>()` 注册 DbContext；支持形状会走 generated-first path。

### 2.2 EF / Repository 已知限制

- Skywalker 的 repository registration path 是 AOT-friendly；full EF Core NativeAOT readiness 仍取决于 EF Core 和具体 provider。
- 不支持的 `DbSet` / entity shape 会报告 `SKY3001`-`SKY3006`，而不是静默跳过。
- 自定义手写 repository 注册不会被 generated defaults 覆盖；迁移期可继续保留手动注册。

### 2.3 已知问题：EF Core 9 + Pomelo 设计时关系模型 NRE（#291）

**状态**：已知问题，调查中；**不阻塞 rc.1**（应用运行时不受影响）

**现象**：继承 `SkywalkerDbContext<TDbContext>` 的项目，在 **EF Core 9.0.x + Pomelo.EntityFrameworkCore.MySql 9.0.0** 下执行任何走设计时关系模型的命令——`dotnet ef migrations add / remove / script`、`database update` 脚手架，或任何访问 `IDesignTimeModel.Model`（→ `GetRelationalModel(designTime:true)`）的路径——会抛 `NullReferenceException`，栈顶在 `RelationalTypeMappingSource.FindCollectionMapping`。

**范围**：仅设计时；应用运行时（CRUD、查询、运行时关系模型）正常。换成不继承 `SkywalkerDbContext` 的普通 `DbContext`、相同实体形状则不复现，所以根因在 `SkywalkerDbContext` 的模型/服务配置（疑点：`OnConfiguring` 中 `ReplaceService<IValueGeneratorSelector, SkywalkerValueGeneratorSelector>()`，以及 `AggregateRoot` 的 `ICollection<object>` 事件字段在设计时被当作 element collection），而非 EF/Pomelo/业务实体本身。

**临时绕过**：用一个**不继承** `SkywalkerDbContext` 的普通 `DbContext`（实体配置等价）来生成迁移脚本，再应用到目标库。

**跟踪**：[#291](https://github.com/dengxuan/Skywalker/issues/291)。

---

## 3. DynamicProxy 拦截器

### 3.1 Castle.DynamicProxy → Source-generated static proxies

**状态**：✅ 已落地（preview.4 track，#264-#270）

**现象**：v1.x 依赖 `Castle.Core` (~600 KB) 在运行时 IL Emit 代理类，AOT 不友好。v1.x 中只要服务实现 `IInterceptable`，`AddInterceptedServices()` 就会尝试用 Castle 在运行时创建接口代理或类代理。

**原因**：v2.0 的 DynamicProxy 支持路径改为编译期生成静态代理。这样可以移除 `Castle.Core` 传递依赖、避免运行时 IL Emit，并让 NativeAOT publish gate 能验证代理路径。

**v2.0 行为**：
- `Castle.Core`、`CastleProxyGenerator`、`IProxyGenerator` 已从 `Skywalker.Extensions.DynamicProxies` 移除。
- `AddDynamicProxies()` 保留为兼容 no-op，不再注册运行时代理工厂。
- `AddInterceptedServices()` 只会在找到 source-generated proxy metadata 时替换服务注册。
- 对带有可拦截接口方法、但缺少 generated proxy metadata 的 legacy Castle-only 注册，启动期会抛出明确迁移错误。
- 对没有接口方法的 marker registration，保持原注册，避免 DDD 自动注册中的空 marker 接口被误判为需要代理。

#### 必需项目引用

消费项目需要同时引用 runtime 包和 source generator analyzer。项目引用写法如下；NuGet 使用时 generator 包也应作为 analyzer/private asset 引入。

```xml
<ItemGroup>
  <PackageReference Include="Skywalker.Extensions.DynamicProxies" Version="2.0.0-preview.*" />
  <PackageReference Include="Skywalker.Extensions.DynamicProxies.SourceGenerators" Version="2.0.0-preview.*" PrivateAssets="all" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
</ItemGroup>
```

仓库内项目引用示例：

```xml
<ProjectReference Include="..\..\src\Skywalker.Extensions.DynamicProxies.SourceGenerators\Skywalker.Extensions.DynamicProxies.SourceGenerators.csproj" OutputItemType="Analyzer" ReferenceOutputAssembly="false" PrivateAssets="all" />
```

#### Before: v1.x Castle-backed interception

```csharp
public interface IOrderService : IInterceptable
{
    Task<string> SubmitAsync(string number);
}

public sealed class OrderService : IOrderService
{
    public Task<string> SubmitAsync(string number)
        => Task.FromResult($"submitted:{number}");
}

services.AddTransient<IOrderService, OrderService>();
services.AddTransient<IInterceptor, AuditInterceptor>();
services.AddInterceptedServices(); // v1.x used Castle at runtime
```

#### After: v2.0 generated interface proxy

```csharp
public interface IOrderService : IInterceptable
{
    Task<string> SubmitAsync(string number);
}

public sealed class OrderService : IOrderService
{
    public Task<string> SubmitAsync(string number)
        => Task.FromResult($"submitted:{number}");
}

services.AddTransient<IOrderService, OrderService>();
services.AddTransient<IInterceptor, AuditInterceptor>();
services.AddInterceptedServices(); // v2.0 uses generated proxy metadata
```

No startup code change is required for supported interface-proxy shapes. The critical migration step is adding the DynamicProxy source generator analyzer to the consuming project and keeping the service shape supported.

#### Supported preview.4 service shape

- Service is registered through an interface, for example `IOrderService -> OrderService`.
- The implementation type is `public` or `internal` in the consuming assembly.
- The implementation implements `IInterceptable` through the service interface or another interface.
- Intercepted methods are ordinary instance interface methods.
- Supported return shapes: `void`, synchronous value, `Task`, `Task<T>`, `ValueTask`, `ValueTask<T>`.
- Supported parameter passing: by-value parameters only.
- Supported generic shape: closed generic service interfaces, such as `IEntityService<string>`.

#### Known limitations in preview.4

- No class-only proxy replacement. Register services through interfaces.
- No `ref`, `out`, or `in` parameters. The generator reports `SKY3101`.
- No generic methods such as `T Echo<T>(T value)` in preview.4. The generator reports `SKY3101`.
- No open generic service registration proxy bridge unless a generated closed shape exists.
- No factory/instance registration proxying when the implementation type cannot be known at compile time.
- `IProxyGenerator` and `CastleProxyGenerator` calls must be removed; there is no runtime factory replacement in v2.0.

#### Migration checklist

1. Remove direct references to `Castle.Core`, `Castle.DynamicProxy`, `CastleProxyGenerator`, and `IProxyGenerator`.
2. Add the DynamicProxy source generator analyzer to each project that declares intercepted services.
3. Keep intercepted services registered as interface-to-implementation descriptors.
4. Move class-only interception to an interface service contract.
5. Replace `ref` / `out` / `in` parameters with by-value request/response models.
6. Replace generic methods with non-generic methods on closed generic service interfaces, or keep them un-intercepted until a later preview supports generic methods.
7. Run the source generator quality workflow or the AOT canary publish command to confirm no proxy-related `IL2xxx` / `IL3xxx` warnings.

#### Diagnostics

`SKY3101` documents unsupported method signatures: [SKY3101](../diagnostics/SKY3101.md).

DI auto-registration diagnostics are documented in the diagnostics index. `SKY1002` covers explicit service types that are not assignable from their implementation: [SKY1002](../diagnostics/SKY1002.md).

---

## 4. EventBus

### 4.1 Handler 发现：反射 → SG

**状态**：延期至 v2.x；不作为 `v2.0.0-rc.1` blocker

**现象**：v1.x 启动期扫描所有程序集找 `ILocalEventHandler<T>` 实现并注册。

**v2.0 口径**：Handler 自动发现 SG 不进入 `v2.0.0-rc.1` release scope。v2.0 用户按现有 EventBus API 迁移；后续 v2.x 如果引入 EventBus SG，会以独立 issue 和迁移条目补充。

### 4.2 领域事件系统统一

**关联 issue**：#155（「重构领域事件系统：统一为显式事件模型」）

该重构已从 `v2.0.0` milestone 移出，保留为后续 v2.x 架构议题，不阻塞 rc.1。

**已知限制（事件持久性，#292）**：当前领域/分布式事件经 `AggregateRoot.AddDistributedEvent(...)` 收集，在**业务事务提交后**由 `IUnitOfWork.OnCompleted` 发布——**非持久**：提交与发布之间进程崩溃或事件总线失败会**丢事件**，无 at-least-once 保证；自动实体变更事件（Created/Updated/Deleted）在本版本**不从 EF 仓储路径发出**。对"绝不能丢"的事件，应用层需自建事务性 outbox。框架内置的事务性 outbox 与显式事件模型规划在 v2.x（[#292](https://github.com/dengxuan/Skywalker/issues/292)、[#155](https://github.com/dengxuan/Skywalker/issues/155)）。

---

## 5. Messaging & Transport → **已独立为 Vertex 项目**

**状态**：✅ 决定落地，代码迁移进行中

**背景**：`Skywalker.Messaging.*` 和 `Skywalker.Transport.*` 四个包本质是**跨语言通信基础设施**（对标 NATS、ZeroMQ），与 Skywalker 的 .NET DDD 框架定位不同步。把它们捆在 .NET 框架里导致 Go/PHP 等语言无法对等接入，复现 "每个语言自己造轮子" 的 bug 模式（这正是 Epic #201 原本想根治的）。

**v2.0 处置**：

1. **4 个包迁出到独立项目 Vertex（polyrepo）**：
    - [`dengxuan/Vertex`](https://github.com/dengxuan/Vertex) — **wire spec / protos / compat 测试**（语言中立）
    - [`dengxuan/vertex-dotnet`](https://github.com/dengxuan/vertex-dotnet) — .NET 实现（NuGet 包 `Vertex.Messaging`、`Vertex.Transport.Grpc` 等）
    - [`dengxuan/vertex-go`](https://github.com/dengxuan/vertex-go) — Go 实现（module `github.com/dengxuan/vertex-go`）
    - Wire spec 文档化，语言中立（见 [wire-format.md](https://github.com/dengxuan/Vertex/blob/main/spec/wire-format.md)）
    - 4 条 transport 铁律：[transport-contract.md](https://github.com/dengxuan/Vertex/blob/main/spec/transport-contract.md)

2. **Skywalker 侧过渡**：
    - `Skywalker.Messaging.*` / `Skywalker.Transport.*` 发终版 `1.0.1`，类型标记 `[Obsolete]` + 带 `[TypeForwardedTo]` 指向新的 `Vertex.*` 命名空间
    - 兜底期约 **3-6 个月**；v2.0 GA 时老包进 EOL
    - Skywalker 不再维护这 4 个包的新功能；Messaging/Transport 相关 PR 一律去 Vertex polyrepo

**迁移步骤**（NuGet 包名**无** `Dotnet` 前缀——生态本身已暗示语言）：

| 你目前用的 | 改为 |
|---|---|
| `<PackageReference Include="Skywalker.Messaging" ... />` | `<PackageReference Include="Vertex.Messaging" ... />` |
| `<PackageReference Include="Skywalker.Transport.Grpc" ... />` | `<PackageReference Include="Vertex.Transport.Grpc" ... />` |
| `<PackageReference Include="Skywalker.Transport.NetMq" ... />` | `<PackageReference Include="Vertex.Transport.NetMq" ... />` |
| `using Skywalker.Messaging;` | `using Vertex.Messaging;` |
| `using Skywalker.Transport.Grpc;` | `using Vertex.Transport.Grpc;` |
| `services.AddGrpcTransport(...)` | `services.AddVertexGrpcTransport(...)` |

API 语义保持不变（`IMessageBus`、`IRpcClient`、`MessagingChannel`、`ITransport`、4 条铁律），**仅**更换命名空间和包名。

**跨语言场景新增**：如果你的服务需要和 Go 服务通信，现在可以用 [`vertex-go`](https://github.com/dengxuan/vertex-go)（`go get github.com/dengxuan/vertex-go/messaging`）—— 与 .NET 共享 wire spec，不再需要自研。

**参考**：
- [Skywalker 内的 spin-out 设计文档](../architecture/messaging-spin-out.md)
- [Epic #201 Feivoo gRPC bidi 消息内核](https://github.com/dengxuan/Skywalker/issues/201) （已更新为以 Vertex 为内核）

---

## 6. 对象映射

### 6.1 AutoMapper 移除

**状态**：✅ 已落地（PR #181，`main`）

**现象**：框架不再内置 AutoMapper 依赖。

**v2.0 态度**：**不会**引入替代方案；推荐 [Mapperly](https://github.com/riok/mapperly)（SG、零运行时、AOT 友好，与 v2.0 哲学一致）。

**迁移步骤**：
1. 在需要映射的项目加 `<PackageReference Include="Riok.Mapperly" Version="..." />`
2. 用 `[Mapper]` partial class 定义映射
3. 详见 Mapperly 官方文档

---

## 7. NativeAOT 零警告验证

**状态**：✅ CI gate 已接入；RC 候选继续验证

v2.0 GA 前的硬门禁：source-generator 覆盖路径的 AOT canary publish 必须 **0 个 trim/AOT 警告**。

验证方法：
```bash
dotnet publish samples/Skywalker.Sample.AspireAOT/Skywalker.Sample.AspireAOT.csproj -c Release /p:PublishAot=true
```

已知边界：EF Core 作为 rich ORM provider 继续保留；EF full NativeAOT 能力取决于 EF Core/provider。NativeAOT-first repository provider（Dapper.AOT / ADO.NET）属于 v2.x 规划，不阻塞 rc.1。

---

## 8. 移除 / 废弃列表

下表在**每次** breaking change 落地时更新：

| 删除的 API / 包 | 在哪个 PR 删除 | 替换方案 |
|---|---|---|
| `AutoMapper` 依赖 | #181（main） | Mapperly |
| `Castle.Core` dependency from `Skywalker.Extensions.DynamicProxies` | #269 | DynamicProxy source generator analyzer + generated interface proxies |
| `CastleProxyGenerator` / `IProxyGenerator` | #269 | Interface service registration covered by `Skywalker.Extensions.DynamicProxies.SourceGenerators` |
| `ISkywalkerBuilder.AddRedisCaching()` / `.AddRabbitMQEventBus()` 重载（含 `Action<Options>` 形式） | #293（PR #301） | 改用 `services.AddRedisCaching()` / `services.AddEventBusRabbitMQ()`（`IServiceCollection` 扩展，未变）。增强模块不再链式挂 `AddSkywalker()`：`services.AddSkywalker(); services.AddRedisCaching();`。原因：这些重载是纯语法糖、却把 `Caching.Redis`/`EventBus.RabbitMQ` 拴到 `Ddd.Abstractions`，破坏“非 DDD 模块可独立安装”原则（Epic #300） |

---

## 9. 常见问题 (FAQ)

> 在 preview 期收到用户反馈后填充。

---

## 10. 贡献本文档

在 `release/2.0` 上落地 breaking change 的 PR 必须：

1. 在对应模块小节下新增条目（若无合适小节则新建）
2. 填齐 **现象 → 原因 → 迁移步骤** 三段
3. 在 [§8 移除 / 废弃列表](#8-移除--废弃列表) 登记 API/包删除
4. PR 描述里写明 `docs/migration/v1-to-v2.md` 已同步

维护者审核 PR 时，**本文档未同步即视为不合格**。

---

## 附录 A：模板

新条目请复制下面的模板：

```md
### X.Y 标题

**状态**：规划中 | ✅ 已落地（PR #N）

**现象**：v1.x 的行为 / API 是什么。

**原因**：为什么在 v2.0 改掉（性能？AOT？一致性？）。

**迁移步骤**：
1. ...
2. ...

**示例代码**：
\`\`\`csharp
// v1.x
...

// v2.0
...
\`\`\`
```
