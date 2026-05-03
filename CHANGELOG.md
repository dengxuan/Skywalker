# Changelog

本项目所有显著变更将记录在本文件中。

格式参考 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.1.0/)，
版本号遵循 [Semantic Versioning](https://semver.org/lang/zh-CN/)。

## [Unreleased]

### Added

- DI auto-registration Source Generator preview.5 首版 registration metadata 输出：属性标注服务会生成 `__SkywalkerDependencyInjectionRegistrar`，支持接口 fallback、显式 `ServiceType`、Scoped/Transient lifetime 映射，并对不可赋值服务类型报告 `SKY1002`（#280）。
- DI auto-registration Source Generator preview.5 runtime bridge：`AddSkywalker()` 现在可通过 assembly metadata 消费生成的 DI registrar，并保留现有 FeatureProvider fallback 路径（#281）。
- DI auto-registration Source Generator preview.5 测试安全网：新增主要 registrar 生成形状的 snapshot 覆盖，以及 generated registration 的解析、Scoped lifetime、idempotency runtime 测试（#282）。
- DI auto-registration Source Generator preview.5 scaffolding：新增 analyzer-only `Skywalker.Ddd.Abstractions.SourceGenerators` 项目、首版 incremental generator skeleton、`SKY1xxx` diagnostics infrastructure 和最小 smoke tests；runtime package 暂不消费该 generator，后续 #280/#281 接入生成注册元数据与 `AddSkywalker()`（#279）。
- DI auto-registration Source Generator preview.5 设计契约：定义 `[Service]` / `[ApplicationService]` / `[Repository]` / `[EventHandler]` attribute model、generated registrar shape、`AddSkywalker()` integration、convention fallback、`SKY1xxx` diagnostic candidates 和 readiness gates（#278）。
- DynamicProxy Source Generator preview.4 迁移与诊断文档：补全 Castle → source-generated static proxy 的 before/after、支持/限制清单、`SKY3101` 修复指南和 preview.4 readiness 链接（#270）。
- DynamicProxies v2.0 移除 Castle.Core runtime fallback：`AddInterceptedServices()` 现在只接受 source-generated static proxy metadata，缺少 generated proxy 的 legacy Castle-only 注册会给出明确迁移错误（#269）。
- `Skywalker.Sample.AspireAOT` 扩展为 DynamicProxy Source Generator AOT canary：在 NativeAOT publish gate 中同时验证 EF repository generated registration 与 DynamicProxy generated static proxy/interceptor 路径零 IL2xxx/IL3xxx warning（#268）。
- DynamicProxy Source Generator preview.4 测试安全网：新增 50 个静态代理 snapshot 场景、generated proxy DI runtime 覆盖，并让 `AddInterceptedServices()` 优先使用生成代理元数据，Castle 继续作为兼容 fallback（#267）。
- DynamicProxy Source Generator preview.4 新增首版接口静态代理生成：为实现 `IInterceptable` 的 public/internal 服务生成同步与 Task/ValueTask 方法代理，并对暂不支持的方法签名报告 `SKY3101` 诊断（#266）。
- DynamicProxy Source Generator preview.4 scaffolding：新增 analyzer-only `Skywalker.Extensions.DynamicProxies.SourceGenerators` 项目和 no-op 候选发现 smoke tests，为后续静态代理生成与 Castle.Core 移除做准备（#265）。
- EF Repository registration 明确 reflection fallback 策略：generated registration 优先；fallback 仅作为 non-AOT 兼容路径保留，并可通过 AppContext switch 禁用以验证 AOT/trimmed 边界（#241）。
- `Skywalker.Sample.NestedTypes` 填充为 EF repository source generator smoke sample，验证 nested DbContext/entity types 的 generated metadata 与 repository/domain-service 注册（#242）。
- EF Repository Source Generator 新增 `SKY3001`-`SKY3006` 诊断，覆盖非 public `DbSet`、不可访问实体、缺少 `IEntity`、抽象实体、重复 `DbSet` 暴露和冲突 key type 推断等不支持场景，并补充对应诊断文档与测试（#239）。
- Sprint 1 EF Repository Source Generator 起步：新增 `Skywalker.Ddd.EntityFrameworkCore.SourceGenerators` 项目，先生成 DbContext `DbSet<TEntity>` 对应的默认 repository/domain service 静态注册代码，`AddSkywalkerDbContext<TDbContext>()` 优先调用 generated registration 并保留运行时反射 fallback，同时补充 generator/runtime 单元测试与 `Skywalker.Sample.Minimal` smoke sample。
- `Skywalker.Sample.MultiDbContext` 填充为 EF repository source generator smoke sample，验证同一应用中两个 DbContext 的 generated registration metadata、registrar 类型和 repository/domain-service 注册彼此独立。
- `Skywalker.Sample.LegacyMigration` 填充为 EF repository source generator 迁移期 smoke sample，验证手写 keyed repository 注册不会被 generated defaults 覆盖，同时缺失的 repository/domain-service 注册会被补齐。
- `Skywalker.Sample.AspireAOT` 填充为 EF repository source generator NativeAOT canary，验证 generated registrar direct-call 合约可发布且不产生 IL2xxx/IL3xxx warnings。
- Source Generator Sprint 0 基础设施：新增 SG 测试项目、`GeneratorTestHelper`、Verify/Roslyn driver 测试流程、`sg-quality.yml` CI、8 个 sample matrix 项目、PublicApiAnalyzers baseline、诊断文档骨架、边界场景清单、`Skywalker.SourceGenerators.Common` 共享库和 `dotnet new skywalker-generator` 模板（#185-#191）。

### Changed — BREAKING (Messaging & Transport 独立为 Vertex 项目)

- **`Skywalker.Messaging.*`** 和 **`Skywalker.Transport.*`** 共 5 个包**从 Skywalker 仓移除**，迁入独立的跨语言项目 [**Vertex**](https://github.com/dengxuan/Vertex)（polyrepo）：
    - .NET 实现：[dengxuan/vertex-dotnet](https://github.com/dengxuan/vertex-dotnet)（NuGet：`Vertex.Messaging`、`Vertex.Messaging.Abstractions`、`Vertex.Transport.Abstractions`、`Vertex.Transport.NetMq`、`Vertex.Transport.Grpc`）
    - Go 实现：[dengxuan/vertex-go](https://github.com/dengxuan/vertex-go)（module `github.com/dengxuan/vertex-go`）
    - Wire 规范：[dengxuan/Vertex](https://github.com/dengxuan/Vertex)
- 原因：Messaging/Transport 本质是**跨语言通信基础设施**，与 Skywalker 的 .NET DDD 框架定位不同步；强行捆绑导致 Go 等语言场景无法对等接入。详见 [messaging-spin-out.md](docs/architecture/messaging-spin-out.md)。
- **迁移**：把 `using Skywalker.Messaging;` / `using Skywalker.Transport;` 改为 `using Vertex.Messaging;` / `using Vertex.Transport;`；`<PackageReference Include="Skywalker.Messaging" />` 改为 `<PackageReference Include="Vertex.Messaging" />` 等。API 语义（`IMessageBus`、`IRpcClient`、`ITransport`、4 条铁律）保持不变。
- **已发布的 `Skywalker.Messaging.1.0.0` 等 NuGet 包保留不动**，老用户可继续 pin 至 `1.0.0`；不会有 `Skywalker.Messaging.1.0.1+` 继续发布。需要新功能 / bug fix 时请升级到 `Vertex.*`。
- _后续_ 可能发布 `Skywalker.Messaging.1.0.1` 等带 `[TypeForwardedTo]` 的桥接包，指向 `Vertex.*`。是否发布、何时发布视下游实际迁移情况而定；目前**不**自动发。

### Changed

- 迁移到 [MinVer](https://github.com/adamralph/minver) 由 git tag 驱动版本号 (#213, #215)。
- 新增权威的版本策略文档 [docs/versioning.md](docs/versioning.md)，从 CONTRIBUTING 链接 (#216)。
- 简化 forward-merge 冲突处理路径，发布工作流增加 `workflow_dispatch` 触发 (#217)。
- CONTRIBUTING / 架构文档同步反映 Messaging/Transport → Vertex 的 spin-out (#218, #219, #221)。

## [2.0.0-preview.1] - 2026-04-23

v2.0 发版通道的第一个标记 tag。基于 v1.0.0 stable，从 `release/2.0` 分支开辟独立发布通道，用于后续 v2.0 全面 Source Generator 化迭代的 preview / rc 浸泡测试。包内容与 v1.0.0 几乎一致，差异仅在版本号体系与新增的迁移指南骨架。

### Added

- v1.x → v2.0 迁移指南骨架 [docs/migration/v1-to-v2.md](https://github.com/dengxuan/Skywalker/blob/release/2.0/docs/migration/v1-to-v2.md)（仅在 `release/2.0` 分支）(#211)。

### Changed

- 版本号体系切换为 `2.0.0-preview.x`（MinVer 自动）。
- CI 跳过 `Versions.props` 回写以避免在 `release/2.0` 上意外触发 main 同步循环 (#212)。

## [1.0.0] - 2026-04-23

Skywalker 首个 stable 版本，提供完整的 .NET 8 DDD 应用开发框架。

### Added — 核心框架

- DDD 基础抽象与 EF Core 集成（`Skywalker.Ddd`、`Skywalker.Ddd.EntityFrameworkCore`）
- 应用服务、Repository、UnitOfWork、领域事件、值对象、聚合根、规约
- 多租户支持、审计日志、对象映射约定、数据过滤

### Added — 模块

- `Skywalker.Permissions.*`：权限注册 / 验证 / 服务端授权数据同步（issues #9-#16）
- `Skywalker.Settings.*` / `Skywalker.Localization` / `Skywalker.Validation`
- `Skywalker.Security` / `Skywalker.AspNetCore`
- `Skywalker.Extensions.*`：`DependencyInjection`、`DynamicProxies`、`Universal`、`Linq` 等

### Added — Source Generator（v1 阶段，反射混合期）

- `Skywalker.SourceGenerators`：模块化自动注册（`SkywalkerModuleAttribute` + 程序集扫描，issues #120-#126）
- `Skywalker.Extensions.DynamicProxies.SourceGenerators`：拦截器静态代理生成
- `AddSkywalker()` / `ISkywalkerBuilder` 一站式注册入口（Console / Web 统一）

### Added — Messaging & Transport（已于 [Unreleased] spin-out 至 Vertex）

- `Skywalker.Messaging.Abstractions` / `Skywalker.Messaging`：bidi 消息内核（`IMessageBus` / `IRpcClient` / `IRpcHandler<TReq,TRes>`）(#193)
- `Skywalker.Transport.Abstractions` / `Skywalker.Transport.NetMq`：传输层抽象 + ZeroMQ 实现 (#193)
- `Skywalker.Transport.Grpc` / `Skywalker.Transport.Grpc.Server`：gRPC bidi 传输实现 (#203, #206, #208)
- 4 条铁律协议约束并通过 XML doc 在 `ITransport.SendAsync` 上固化 CT 契约 (#202, #205)
  1. read loop 只内联路由 Ack；handler 异步派发
  2. 单条 reply 发送失败 ≠ 断连
  3. write 拿到锁后必须不被 CT 中断（CT 只用于「上线前」取消）
  4. 唯一断连源是 read loop 的 `Recv()` / `MoveNext()` 错误

### Changed — BREAKING（v0.x 用户）

- **移除 `Skywalker.Ddd.Application` 对 [AutoMapper](https://github.com/AutoMapper/AutoMapper) 的依赖**（AutoMapper 自 v15 起转为商业授权）(#181)。
  - 删除 `SkywalkerProfile`。
  - 移除 `ApplicationService` 的 `IMapper Mapper` 属性与构造函数参数。
  - 移除 `eng/Versions.props` 中的 `AutoMapperVersion`。
- `ApplicationService` 改为无参基类，业务派生类自行注入所需服务。
- 全部文档示例改为使用 [Riok.Mapperly](https://github.com/riok/mapperly) 源生成器。

### Documentation

- 完整 README / CONTRIBUTING / API / 使用指南 / 示例项目（issues #17-#21, #66）
- 架构设计文档体系（[docs/architecture/](docs/architecture/)）
- v2.0 Source Generator 路线图与规范（[v2.0-roadmap.md](docs/architecture/v2.0-roadmap.md)、[source-generators-spec.md](docs/architecture/source-generators-spec.md)、[source-generators-quality.md](docs/architecture/source-generators-quality.md)）(#183)

### v0.x → v1.0 Migration Guide（AutoMapper → Mapperly）

派生 `ApplicationService` 的业务代码需要按以下步骤迁移：

1. **移除构造函数中的 `IMapper` 参数**

   ```diff
   - public OrderAppService(IMapper mapper, IOrderRepository repo) : base(mapper)
   + public OrderAppService(IOrderRepository repo)
     {
         _repo = repo;
     }
   ```

2. **业务项目添加 Mapperly 包引用**

   ```xml
   <PackageReference Include="Riok.Mapperly" Version="4.1.1" />
   ```

3. **将 `Profile` 改为 `[Mapper] partial class`**

   ```diff
   - public class OrderProfile : Profile
   - {
   -     public OrderProfile() => CreateMap<Order, OrderDto>();
   - }
   + [Mapper]
   + public partial class OrderMapper
   + {
   +     public partial OrderDto ToDto(Order entity);
   +     public partial List<OrderDto> ToDtoList(IEnumerable<Order> entities);
   + }
   ```

4. **替换调用点**

   ```diff
   - return Mapper.Map<OrderDto>(order);
   + return _mapper.ToDto(order);
   ```

5. **移除 DI 中对 AutoMapper 的注册**（如有）：删除 `services.AddAutoMapper(...)`。

理由：

- AutoMapper 自 v15 起转向商业授权，框架不应将商业许可成本传递给使用者。
- Mapperly 是 MIT 许可的源生成器，编译期生成映射代码，零运行时反射开销，对原生 AOT 友好，重构时编译器即可发现字段不匹配。
- 框架原本就未在 DI 容器中注册 AutoMapper，派生 `ApplicationService` 的服务实际无法解析 `IMapper`，移除此依赖同时修复该潜在缺陷。

---

## v2.0 Roadmap

v2.0 是 Skywalker 的差异化战役，定位 **小、快、易用**：

- 全面 Source Generator 化：消灭运行时反射、消灭 `Reflection.Emit`。
- 移除 `Castle.Core` 依赖（~600 KB）。
- NativeAOT publish 零警告。
- `AddSkywalker()` 一行启动，零样板注册。
- Messaging/Transport 已 spin-out 到 [Vertex](https://github.com/dengxuan/Vertex) 独立项目。

设计与规划文档：

- [`docs/architecture/v2.0-roadmap.md`](docs/architecture/v2.0-roadmap.md)
- [`docs/architecture/source-generators-spec.md`](docs/architecture/source-generators-spec.md)
- [`docs/architecture/source-generators-quality.md`](docs/architecture/source-generators-quality.md)

发版策略：`release/2.0` → `2.0.0-preview.x` → `2.0.0-rc.x` → `2.0.0`，每阶段最低浸泡 2 周，详见 [versioning.md](docs/versioning.md)。

[Unreleased]: https://github.com/dengxuan/Skywalker/compare/v2.0.0-preview.1...HEAD
[2.0.0-preview.1]: https://github.com/dengxuan/Skywalker/compare/v1.0.0...v2.0.0-preview.1
[1.0.0]: https://github.com/dengxuan/Skywalker/releases/tag/v1.0.0
