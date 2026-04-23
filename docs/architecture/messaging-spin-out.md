# Messaging / Transport spin-out to Vertex

> 状态：**决议已定**　|　日期：2026-04-23　|　影响：Skywalker v2.0 范围

本文档记录一个架构级决定：把目前在 Skywalker 仓里的 `Skywalker.Messaging.*` 和 `Skywalker.Transport.*` 四个包**剥离出去**，成立独立项目 **Vertex**。

---

## 决定

**Vertex** 是一个独立的、跨语言的 bidi messaging kernel，**不再作为 Skywalker 的一部分**。

- **定位**：轻量级跨语言消息内核，对标 NATS / ZeroMQ，不是 .NET 框架的一部分
- **仓**：独立 GitHub 仓 `dengxuan/Vertex`，monorepo 结构包含 `/dotnet`、`/go`、`/protos`、`/spec`
- **初始语言**：.NET 和 Go 并行（无主次）；未来根据需要加 PHP / Rust / Python
- **Skywalker 的关系**：Skywalker 作为**消费者**（如果需要消息总线）通过 NuGet 依赖 `Vertex.Dotnet.*`；不再捆绑

## 为什么必须拆

Skywalker 当前把两类完全不同的东西捆在了一起：

| 子系统 | 本质 | 跨语言吗？ | 受众 |
|---|---|---|---|
| DDD / Caching / Permissions / Localization / ... | **.NET 应用内架构模式**（C# 语言特性深度绑定）| ❌ 天然 .NET-only | C# 开发者 |
| **Messaging + Transport** | **跨进程通信基础设施** | ✅ 应该跨语言 | 任何后端工程师 |

把这两类放在同一个 .NET 框架里，Messaging/Transport 永远会被"拉进"C# 生态（MessagePack 写死、`Type.FullName` 作 topic、DI registration 是 `IServiceCollection`...），跨语言对接需要重新发明轮子。

现实验证：`gaming-dotnet-sdk` 和 `gaming-go-sdk` 在 v1.3.0 之前都各自手写了一套 gRPC bidi dispatch，**踩了 3 个同源 bug**（见 Epic #201）。当前方案虽然在 .NET 侧把 bug 治住了，但 Go 侧还没动 —— 本质原因就是 Skywalker 是 .NET 框架，无法在 Go 上线。

把通信层独立为 Vertex 后，.NET 和 Go 获得的是**同一个项目的两个语言实现**，共享 wire spec + 测试矩阵，不再是"两个各写各的系统"。

## 范围：什么迁、什么留

### 迁出到 Vertex

| Skywalker 里的包 | Vertex 里的对应 |
|---|---|
| `Skywalker.Messaging.Abstractions` | `Vertex.Dotnet.Messaging.Abstractions` |
| `Skywalker.Messaging` | `Vertex.Dotnet.Messaging` |
| `Skywalker.Transport.Abstractions` | `Vertex.Dotnet.Transport.Abstractions` |
| `Skywalker.Transport.NetMq` | `Vertex.Dotnet.Transport.NetMq` |
| `Skywalker.Transport.Grpc` | `Vertex.Dotnet.Transport.Grpc` |

加上 Go 并行实现、wire spec 文档、envelope protos。

### 留在 Skywalker

所有 DDD / Caching / Permissions / Localization / Settings / Validation / Template / EventBus.Local / RateLimiters / SMS / HealthChecks / Ddd.EntityFrameworkCore / ... 模块保持不变。Skywalker 继续作为 .NET DDD 框架演进（Epic #182 v2.0 Source Generator 化）。

## 过渡方案

已发布到 GitHub Packages 的旧包（`Skywalker.Messaging.*` 1.0.0 / 2.0.0-preview.1.3 等）需要兼容过渡：

| 阶段 | 动作 |
|---|---|
| T0 | Vertex 仓建立，迁代码，发 `Vertex.Dotnet.*` 1.0.0 |
| T0 | Skywalker 发 `Skywalker.Messaging.*` / `Skywalker.Transport.*` 的**最终版 1.0.1**：全部类型 `[Obsolete]` + `[TypeForwardedTo]` 指向 Vertex；项目不再收新 commit |
| T0 | `Skywalker.sln` 移除这 4 个项目 |
| T0 | `docs/migration/v1-to-v2.md` 新增 "Messaging/Transport spun out to Vertex" 大条目 |
| T0 | 更新 Epic #201：Phase 2/3 的 NuGet 包名全部改为 `Vertex.*` |
| T + v2.0 GA | Skywalker 老 `Messaging.*` / `Transport.*` 包进 EOL |

`[TypeForwardedTo]` 兜底期约 **3-6 个月**，足够下游切包名。

## 时间线

| 里程碑 | 期望时间 |
|---|---|
| Vertex 仓 bootstrap（README、spec、protos、目录结构、CI） | 本周 |
| `Vertex.Dotnet.*` 代码迁移（从 Skywalker 平移） | 下周 |
| Go 实现 minimum-viable（`transport/grpc` 客户端，能跑通 hello-world）| 2-3 周内 |
| `.NET ↔ Go` 端到端 compat 测试矩阵 | 3 周内 |
| Vertex 1.0.0 GA | 与 Skywalker v2.0 GA 同期或略早 |

## 与其他 epic / 路线图的联动

| 受影响的 | 怎么改 |
|---|---|
| [Epic #201] Skywalker 作为 Feivoo gRPC SDK 统一 bidi 消息内核 | **更新**：把 "Skywalker 作为内核" 改为 "**Vertex** 作为内核，Skywalker 消费 Vertex"。Phase 2-3 的 NuGet 包名从 `Skywalker.*` 改为 `Vertex.Dotnet.*` |
| [Epic #201] Phase 4 Go 镜像（原"暂缓"） | **升级**：不再 "暂缓"；作为 Vertex 1.0 的**核心交付物**与 .NET 并行开发 |
| [Epic #182] v2.0 Source Generator 化 | **不受影响**；Skywalker v2.0 不再包含 Messaging，SG 化工作聚焦 DDD 模块 |
| `docs/versioning.md` | Vertex 有自己的版本线（`Vertex.Dotnet.*` + `vertex-go/v*`），**不与 Skywalker 版本号绑定** |

## 开放决策

以下由 Vertex 项目启动后再定：

- PHP 支持时机（需要时再做）
- `Vertex.Dotnet.*` 1.0.0 的 wire format 基线（4-frame envelope 保留 vs 立刻上 proto envelope）
- Vertex 自己的 CONTRIBUTING / 版本策略（preview → rc → GA 节奏）

## 相关

- [Vertex 仓](https://github.com/dengxuan/Vertex)（创建后更新此链接）
- [Epic #201 Skywalker 作为 Feivoo gRPC SDK 统一 bidi 消息内核](https://github.com/dengxuan/Skywalker/issues/201) — 需要按本文档 update
- [`docs/migration/v1-to-v2.md`](../migration/v1-to-v2.md) — 需要新增条目
- [`docs/modules/transport.md`](../modules/transport.md) — 4 条 transport 铁律，将被复制到 Vertex 的 spec 文档并保留在这里作为历史参考
