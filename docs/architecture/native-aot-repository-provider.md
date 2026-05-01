# NativeAOT Repository Provider 计划

> 状态：**规划中 (Planned)**　|　适用：v2.x　|　最后更新：2026-05-01

本文定义 Skywalker 在 EF Core 无法完全 NativeAOT ready 时的仓储层替代路线。
核心判断：**EF Core 继续作为 rich ORM provider；NativeAOT 应用需要一条独立、可验证、可承诺的 repository provider 路线。**

## 1. 背景

Sprint 1 的 `Skywalker.Sample.AspireAOT` 已验证 EF repository source generator 的 direct-call registration contract 可以 NativeAOT publish，且不产生 IL2xxx / IL3xxx warnings。

这个 canary 证明的是 Skywalker 生成注册链路本身 AOT-friendly，不等价于证明完整 EF Core runtime、EF provider、change tracking、LINQ translation 与模型构建都已经 NativeAOT ready。

因此 v2 需要把两个目标拆开：

| 目标 | 说明 | 发布口径 |
|---|---|---|
| EF Repository SG | 保留 EF Core 作为主流 ORM provider，消除 Skywalker 仓储注册反射 | AOT-friendly registration path |
| NativeAOT Repository Provider | 为 NativeAOT-first 应用提供非 EF 的仓储实现 | AOT-ready repository path |

## 2. 设计原则

1. **不让 EF Core 的 AOT 状态阻塞 Skywalker 的 NativeAOT story**。
2. **仓储抽象稳定，provider 可替换**：`IRepository<TEntity, TKey>` 继续作为领域层契约。
3. **优先采用成熟底座**：先研究 Dapper.AOT / ADO.NET，而不是立刻自研完整 micro ORM。
4. **真实 AOT gate 必须连接真实数据库路径**：Dapper/ADO.NET canary 不能只做 stub。
5. **能力边界要清晰**：NativeAOT provider 不承诺 EF 等价能力，不承诺完整 LINQ/specification translation。

## 3. Provider 分层

### 3.1 EF Core Provider

EF Core 继续承担 rich ORM 场景：

- LINQ 查询与 provider translation
- change tracking
- navigation loading
- migrations
- provider ecosystem

v2 的 EF 目标是：

- repository/domain-service registration generated-first
- AOT/trimmed 场景下避免 Skywalker 自己引入新的 reflection warning
- 文档清楚标注 full EF Core NativeAOT 取决于 EF Core 与具体 provider

### 3.2 Dapper.AOT / ADO.NET Provider

Dapper.AOT / ADO.NET 是 NativeAOT repository path 的优先候选：

- ADO.NET 提供最小运行时依赖面
- Dapper.AOT 可通过 source generation / interceptors 减少传统 Dapper 的 runtime reflection/materialization 成本
- SQL 显式、行为可预期，更适合 NativeAOT CI gate

候选包形态：

| 包 | 角色 |
|---|---|
| `Skywalker.Ddd.Dapper` | Dapper/Dapper.AOT repository runtime provider |
| `Skywalker.Ddd.Dapper.SourceGenerators` | 生成 repository registration、mapping 或 CRUD helper |
| `Skywalker.Sample.DapperAOT` | 真实 NativeAOT CRUD canary |

是否最终命名为 `Dapper` 还是更底层的 `AdoNet`，由 spike 结果决定。若 Dapper.AOT 限制较多，则先落 `Skywalker.Ddd.AdoNet`，再提供 Dapper integration。

## 4. API 草案

### 4.1 显式 provider 注册

```csharp
services.AddSkywalkerEntityFrameworkCoreRepositories<MyDbContext>();
services.AddSkywalkerDapperRepositories<MyConnection>();
```

### 4.2 统一入口注册

```csharp
services.AddSkywalkerRepositories(options =>
{
    options.UseEntityFrameworkCore<MyDbContext>();
});

services.AddSkywalkerRepositories(options =>
{
    options.UseDapper<MyConnection>();
});
```

首个实现优先选择显式 provider 注册，降低 API 面和迁移风险。统一入口可在 provider 能力稳定后收口。

## 5. 阶段计划

### Phase A：Research Spike

目标：判断 Dapper.AOT 是否适合作为 Skywalker NativeAOT repository provider 底座。

- [ ] 创建最小 Dapper.AOT 实验 sample
- [ ] SQLite NativeAOT publish smoke
- [ ] 验证 `Insert` / `GetById` / `Update` / `Delete`
- [ ] 验证 transaction 与 cancellation token
- [ ] 记录 Dapper.AOT 对参数绑定、materialization、nullable、value object 的限制
- [ ] 记录 IL2xxx / IL3xxx / IL305x warnings 来源

产出：spike 报告，决策 `Dapper.AOT first` 或 `ADO.NET first`。

### Phase B：Minimal Provider

目标：提供最小可用的 NativeAOT 仓储实现。

- [ ] `IRepository<TEntity, TKey>.GetAsync`
- [ ] `InsertAsync`
- [ ] `UpdateAsync`
- [ ] `DeleteAsync`
- [ ] unit of work / transaction 的最小桥接
- [ ] `Skywalker.Sample.DapperAOT` 使用 SQLite 跑真实 CRUD
- [ ] CI 强制 `dotnet publish -p:PublishAot=true` 且无 IL2xxx / IL3xxx

非目标：复杂 LINQ、navigation loading、migrations、完整 specification translation。

### Phase C：Source-Generated Mapping

目标：用 source generator 降低样板代码和 runtime reflection。

- [ ] 根据 entity metadata 生成 table/column mapping
- [ ] 生成基础 CRUD SQL
- [ ] 生成 repository registration
- [ ] 对不支持的实体形状给 SKY diagnostics
- [ ] snapshot tests 覆盖 key、nullable、ignored property、constructor edge cases

### Phase D：Specification Subset

目标：只支持 NativeAOT provider 可稳定承诺的查询子集。

- [ ] `Id == value`
- [ ] 简单 `And` / `Or`
- [ ] paging / ordering
- [ ] 明确 unsupported expression diagnostics

复杂 expression translation 不进入 v2.0 stable gate。

## 6. CI Gate

NativeAOT repository provider 的 CI gate 必须比 EF direct-call canary 更真实：

```text
Build provider
Run CRUD tests
Publish Skywalker.Sample.DapperAOT with PublishAot=true
Fail on warning IL2xxx / IL3xxx
Run published binary against SQLite database
```

首个 gate 推荐 SQLite，因为它无需容器即可覆盖真实 DB connection、command、reader、materialization。

SQL Server / MySQL / PostgreSQL 可作为后续 matrix，不作为第一版 blocker。

## 7. 发布口径

### v2.0

- EF Core remains the rich ORM provider.
- EF repository registration is generated-first and AOT-friendly.
- Full EF Core NativeAOT readiness depends on EF Core and provider support.
- NativeAOT repository provider is planned as a separate v2.x track.

### v2.x Preview

- Dapper.AOT / ADO.NET provider becomes the recommended NativeAOT persistence path once the real CRUD AOT gate is green.
- EF Core AOT experiments remain documented but do not block the NativeAOT repository provider.

## 8. 风险

| 风险 | 影响 | 缓解 |
|---|---|---|
| Dapper.AOT API 或 interceptor 约束变化 | provider 设计返工 | 先 spike，不在 Sprint 1 承诺实现 |
| SQL 生成能力被误解为 EF 等价 | 用户预期偏差 | 文档明确 NativeAOT provider 是显式 SQL / CRUD-first |
| Specification translation 成本失控 | 延误 v2 | 只支持明确子集，其余 diagnostic |
| 多数据库 SQL 方言差异 | provider 复杂度上升 | SQLite first，其他 provider matrix 后置 |
| 事务/UoW 语义与 EF 不一致 | 行为差异 | 最小桥接先覆盖 transaction boundary，复杂行为文档化 |

## 9. 决策记录

| 日期 | 决策 | 备注 |
|---|---|---|
| 2026-05-01 | EF Core 与 NativeAOT repository provider 分线推进 | EF Core 不再阻塞 Skywalker NativeAOT 仓储层目标 |
| 2026-05-01 | Dapper.AOT / ADO.NET 作为 NativeAOT provider 首选研究方向 | 先 spike，后决定 `Dapper` 或 `AdoNet` 包形态 |