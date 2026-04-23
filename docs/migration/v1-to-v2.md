# Skywalker v1.x → v2.0 迁移指南

> 状态：**草案 (Living Document)**　|　目标版本：**v2.0.0**　|　最后更新：2026-04-23

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
| ~~`Skywalker.Messaging.*` / `Skywalker.Transport.*`~~ | 1.0.1（最终版，带 `[TypeForwardedTo]`）| —（已迁出）| **迁移到独立项目 [Vertex](https://github.com/dengxuan/Vertex)** —— 改用 `Vertex.Dotnet.*` 包；详见 §5 |
| `Skywalker.EventBus.*` | 1.x | 2.0.0 | 基本兼容（Handler 自动发现改为 SG） |
| `Skywalker.Caching.*` / `Skywalker.Localization.*` / `Skywalker.Permissions.*` / `Skywalker.Settings.*` | 1.x | 2.0.0 | 基本兼容 |

### 升级路径

1. **试 preview 包**：`2.0.0-preview.N`（来自 `release/2.0` 分支自动发布）
2. **对照本文档**逐模块迁移
3. **必选步骤**：在项目根添加 `<EnableSkywalkerSourceGenerator>true</EnableSkywalkerSourceGenerator>`（具体 property 名待 Sprint 1 确认）
4. 跑测试，确认无反射残留（参考 [`3. NativeAOT 零警告验证`](#7-nativeaot--零警告验证)）
5. 发现遗漏请提 issue 并打标签 `migration`

---

## 1. DI 服务注册

### 1.1 `AddSkywalker()` 统一入口

**状态**：规划中（Sprint 3）

**现象**：v1.x 需要多个 `AddXxx()` 调用散落各处，启动代码冗长。

**v2.0 目标**：

```csharp
// v1.x
builder.Services.AddSkywalker()
    .AddAspNetCore();
builder.Services.AddSkywalkerDbContext<AppDbContext>(...);
builder.Services.AddEventBusLocal();
builder.Services.AddRedisCaching(...);

// v2.0（规划）
builder.Services.AddSkywalker<Startup>(cfg =>
{
    cfg.UseEntityFrameworkCore<AppDbContext>(options => options.UseMySql(...));
    cfg.UseLocalEventBus();
    cfg.UseRedisCaching(...);
});
```

**迁移步骤**：待 API 冻结后补充。

---

## 2. Repository 与 EF Core 集成

### 2.1 仓储注册反射 → Source Generator

**状态**：规划中（Sprint 1）

**现象**：v1.x 在启动期用 `MakeGenericType` + 反射为每个实体注册 `IRepository<T, TKey>` 和 `IDbSet<T>` 相关服务。

**v2.0 目标**：
- 在 `DbContext` 内由 SG 扫描 `DbSet<T>` 属性，编译期生成注册代码。
- `IRepository<Order, Guid>` 的具体实现类由 SG 生成，**零运行时反射、AOT 友好**。

**迁移步骤**（占位）：
- [ ] 实体必须加 `partial` 标记？待 spec 确认
- [ ] 自定义 `IOrderRepository : IRepository<Order, Guid>` 是否需要调整？待 spec 确认

### 2.2 其他预留小节

- [ ] **2.2** 自定义 Repository 扩展的注册方式
- [ ] **2.3** Specification 用法是否有变化
- [ ] **2.4** UnitOfWork 拦截器的换装

---

## 3. DynamicProxy 拦截器

### 3.1 Castle.DynamicProxy → SG 静态代理

**状态**：规划中（Sprint 2，v2.0 最大单体工作量）

**现象**：v1.x 依赖 `Castle.Core` (~600 KB) 在运行时 IL Emit 代理类，AOT 不友好。

**v2.0 目标**：
- 完全移除 `Castle.Core` 依赖。
- 由 SG 在编译期生成静态代理类型。
- 新 API：`[Intercept<TInterceptor>]` 强类型拦截器标注。

**迁移步骤**（占位）：
- [ ] 自定义 `IAsyncInterceptor` 实现的迁移
- [ ] 异步 / 泛型 / `ref`-`out` 方法的 edge case
- [ ] AOT publish 零警告验证方法

---

## 4. EventBus

### 4.1 Handler 发现：反射 → SG

**状态**：规划中（Sprint 3 之后）

**现象**：v1.x 启动期扫描所有程序集找 `ILocalEventHandler<T>` 实现并注册。

**v2.0 目标**：SG 编译期收集 handler 元数据；启动时无反射扫描。

**迁移步骤**（占位）

### 4.2 领域事件系统统一

**关联 issue**：#155（「重构领域事件系统：统一为显式事件模型」）

待 PR 落地后填充。

---

## 5. Messaging & Transport → **已独立为 Vertex 项目**

**状态**：✅ 决定落地，代码迁移进行中

**背景**：`Skywalker.Messaging.*` 和 `Skywalker.Transport.*` 四个包本质是**跨语言通信基础设施**（对标 NATS、ZeroMQ），与 Skywalker 的 .NET DDD 框架定位不同步。把它们捆在 .NET 框架里导致 Go/PHP 等语言无法对等接入，复现 "每个语言自己造轮子" 的 bug 模式（这正是 Epic #201 原本想根治的）。

**v2.0 处置**：

1. **4 个包迁出到独立项目 [Vertex](https://github.com/dengxuan/Vertex)**：
    - monorepo 结构（`/dotnet` + `/go` + `/protos` + `/spec`），.NET 和 Go 并行开发
    - Wire spec 文档化，语言中立（见 [Vertex wire-format.md](https://github.com/dengxuan/Vertex/blob/main/spec/wire-format.md)）
    - 4 条 transport 铁律 port 为 [Vertex transport-contract.md](https://github.com/dengxuan/Vertex/blob/main/spec/transport-contract.md)

2. **Skywalker 侧过渡**：
    - `Skywalker.Messaging.*` / `Skywalker.Transport.*` 发终版 `1.0.1`，类型标记 `[Obsolete]` + 带 `[TypeForwardedTo]` 指向 `Vertex.Dotnet.*`
    - 兜底期约 **3-6 个月**；v2.0 GA 时老包进 EOL
    - Skywalker 不再维护这 4 个包的新功能；Messaging/Transport 相关 PR 一律去 Vertex 仓

**迁移步骤**：

| 你目前用的 | 改为 |
|---|---|
| `<PackageReference Include="Skywalker.Messaging" ... />` | `<PackageReference Include="Vertex.Dotnet.Messaging" ... />` |
| `<PackageReference Include="Skywalker.Transport.Grpc" ... />` | `<PackageReference Include="Vertex.Dotnet.Transport.Grpc" ... />` |
| `<PackageReference Include="Skywalker.Transport.NetMq" ... />` | `<PackageReference Include="Vertex.Dotnet.Transport.NetMq" ... />` |
| `using Skywalker.Messaging;` | `using Vertex.Dotnet.Messaging;` |
| `using Skywalker.Transport.Grpc;` | `using Vertex.Dotnet.Transport.Grpc;` |
| `services.AddGrpcTransport(...)` | `services.AddVertexGrpcTransport(...)` |

API 语义保持不变（`IMessageBus`、`IRpcClient`、`MessagingChannel`、`ITransport`、4 条铁律），**仅**更换命名空间和包名。

**跨语言场景新增**：如果你的服务需要和 Go / 其他语言服务通信，现在可以用同一个 Vertex 项目的 [Go implementation](https://github.com/dengxuan/Vertex/tree/main/go)（与 .NET 共享 wire spec），不再需要自研。

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

**状态**：规划中（v2.0 收尾）

v2.0 GA 前的硬门禁：`dotnet publish -c Release /p:PublishAot=true` 必须 **0 个 trim/AOT 警告**。

验证方法（预留章节）：
```bash
# 占位
dotnet publish samples/AotConsoleSample -c Release /p:PublishAot=true
```

---

## 8. 移除 / 废弃列表

下表在**每次** breaking change 落地时更新：

| 删除的 API / 包 | 在哪个 PR 删除 | 替换方案 |
|---|---|---|
| `AutoMapper` 依赖 | #181（main） | Mapperly |
| *（待后续 PR 填充）* | — | — |

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
