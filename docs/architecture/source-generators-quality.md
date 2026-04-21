# Skywalker Source Generator 质量保证规范

> 状态：**草案 (Draft)**　|　适用：v2.0+　|　最后更新：2026-04-21

本规范定义 Skywalker 所有 SG 的质量基线和发版准入标准。
设计规范见 [`source-generators-spec.md`](./source-generators-spec.md)。

## 1. 八层质量保证体系

### Layer 1: Snapshot Test（快照测试）—— 核心防线

**工具**: [`Verify.SourceGenerators`](https://github.com/VerifyTests/Verify.SourceGenerators)

**做法**: 每个生成场景都有一个 `.verified.cs` 文件入库。任何生成代码变化，CI 立即失败。

```csharp
[Fact]
public Task AppService_WithGenericRepository_ShouldGenerate()
{
    var source = """
        public partial class OrderAppService : IOrderAppService
        {
            public Task<Order> GetAsync(Guid id) => default!;
        }
        """;
    return Verify(GeneratorTestHelper.Run<AppServiceGenerator>(source));
}
```

**要求**：
- 每个 SG ≥ 30 个 snapshot。
- `.verified.cs` 必须 PR 中人工审查通过后才能入库。
- 任何 `.received.cs` 出现在 CI = 失败。

### Layer 2: Edge Case 测试矩阵

**做法**: 在 `tests/SourceGenerators/EDGE_CASES.md` 维护"边界场景清单"，每加一个 case 打勾。

强制覆盖的边界场景（节选）：

```
[ ] 普通 class
[ ] partial class（多文件分布）
[ ] record / record struct
[ ] sealed / abstract class
[ ] generic class（开放/闭合）
[ ] nested class
[ ] internal / public 可见性
[ ] file-scoped class
[ ] 全局命名空间（无 namespace）
[ ] file-scoped namespace
[ ] 多接口实现
[ ] 接口继承链
[ ] 跨程序集基类
[ ] async / ValueTask / IAsyncEnumerable
[ ] ref / in / out 参数
[ ] ref struct 参数（应报警告）
[ ] 显式接口实现
[ ] static 方法
[ ] default parameter values
[ ] params 数组
[ ] 泛型方法 T : new()
[ ] attribute 转发
[ ] global using 影响
[ ] using alias
```

完整清单见 `tests/SourceGenerators/EDGE_CASES.md`。

### Layer 3: Diagnostic 优先于沉默

**铁律**: SG 遇到不支持的场景，**绝不能默默不生成**。必须报 `Diagnostic`。

```csharp
// ❌ 禁止
if (!IsSupported(symbol)) return;

// ✅ 必须
if (!IsSupported(symbol))
{
    context.ReportDiagnostic(Diagnostic.Create(
        SkyDiagnostics.RefLikeNotSupported,
        symbol.Locations.FirstOrDefault(),
        symbol.Name));
    return;
}
```

**理由**: 用户能接受功能不全，但接受不了"代码写得好好的，运行起来才发现没注册"。

### Layer 4: Sample 项目矩阵 + 集成测试

`samples/` 不是一个 demo，而是**故意造各种刁钻场景**的项目集：

```
samples/
├── Skywalker.Sample.Minimal/           最简场景
├── Skywalker.Sample.MultiDbContext/    多个 DbContext
├── Skywalker.Sample.GenericServices/   泛型服务
├── Skywalker.Sample.NestedTypes/       嵌套类型
├── Skywalker.Sample.InternalServices/  internal 可见性
├── Skywalker.Sample.Modular/           跨程序集模块
├── Skywalker.Sample.AspireAOT/         NativeAOT publish 验证
├── Skywalker.Sample.LegacyMigration/   v1 → v2 迁移场景
└── Skywalker.Sample.RealWorldShop/     完整电商样例
```

**CI 必须执行**：
1. 所有 sample `dotnet build` 成功
2. 所有 sample 跑端到端集成测试
3. AOT sample `dotnet publish -p:PublishAot=true` **零警告**
4. 启动一次实际请求，验证生成代码真的工作

### Layer 5: Downstream 项目验证

CI 矩阵中纳入**真实 downstream 项目**编译验证：
- Skywalker 自身的内部使用项目
- Fork 的 ABP 改造样本
- 种子用户的项目（征得同意后）

每个 PR 在合并前必须 downstream 全绿。

### Layer 6: Public API 锁定

工具：[`Microsoft.CodeAnalysis.PublicApiAnalyzers`](https://github.com/dotnet/roslyn-analyzers/blob/main/src/PublicApiAnalyzers/PublicApiAnalyzers.Help.md)

```
Skywalker.Xxx/
├── PublicAPI.Shipped.txt    ← 已发布的 API 锁定
└── PublicAPI.Unshipped.txt  ← 待发布的新增 API
```

任何破坏性 API 变更必须显式更新这两个文件，PR review 必看。

### Layer 7: Beta / Preview 通道

**绝不允许** main → 直接 stable。强制走：

```
main 提交              → -alpha 包到 GitHub Packages
打 v2.0-preview tag    → -preview 包到 NuGet（≥ 2 周浸泡）
打 v2.0-rc tag         → -rc 包（≥ 2 周浸泡）
打 v2.0 tag            → stable 到 NuGet
```

每个阶段最少 2 周浸泡。早期 stable 用户用不到，但邀请 ≥ 3 个种子用户用 preview 反馈。

### Layer 8: 专用 Issue 模板

`.github/ISSUE_TEMPLATE/sg-bug.md` 强制要求用户提供：

- 最小复现代码
- 期望的生成代码
- **实际生成的代码**（贴 `obj/Generated/` 中的 `.g.cs`）
- Skywalker 版本、.NET 版本、IDE

让维护者**秒定位问题**。

## 2. 量化质量阈值

### 2.1 单 SG 准入门槛（合并到 main）

| 指标 | 阈值 |
|---|---|
| 单元测试覆盖率 | ≥ 85% |
| Snapshot 测试场景 | ≥ 30 |
| EDGE_CASES 相关项覆盖 | 100% |
| 关联 sample 项目 | ≥ 1 |
| 关联 SKYxxxx 诊断 | ≥ 5 |
| 每个诊断有文档页 | ✅ |
| AOT sample 零警告 | ✅ |
| `PublicAPI.Unshipped.txt` 更新 | ✅ |

### 2.2 Stable 发版准入清单

打 `v2.0.0` 前必须满足全部：

| 类别 | 指标 | 阈值 |
|---|---|---|
| **测试** | 全 SG 累计 snapshot 数 | ≥ 150 |
| | 全 SG 累计 unit test 数 | ≥ 300 |
| | 测试覆盖率 | ≥ 90% |
| **样例** | sample 项目数 | ≥ 8 |
| | 全部 sample CI 全绿 | ✅ |
| | AOT publish 零警告 | ✅ |
| **诊断** | 累计 SKYxxxx 数量 | ≥ 30 |
| | 每个诊断有文档页 | 100% |
| **API** | `PublicAPI.Shipped.txt` 完整 | ✅ |
| **浸泡** | preview 浸泡时长 | ≥ 2 周 |
| | rc 浸泡时长 | ≥ 2 周 |
| | 已知 issue P0/P1 数 | 0 |
| **文档** | 每个 attribute 有示例 | 100% |
| | 迁移指南完整 | ✅ |
| **种子用户** | 试用 preview 的项目数 | ≥ 3 |

**任意一项不达标，禁止打 stable tag。**

## 3. CI Pipeline 规范

GitHub Actions 必备 workflow：

```yaml
jobs:
  build:
    # restore + build + unit test + snapshot test
  
  samples:
    needs: build
    strategy:
      matrix:
        sample: [Minimal, MultiDbContext, GenericServices, NestedTypes,
                 InternalServices, Modular, LegacyMigration, RealWorldShop]
    # 每个 sample build + integration test
  
  aot-publish:
    needs: build
    # AspireAOT sample 跑 dotnet publish -p:PublishAot=true
    # 任何 IL2xxx / IL3xxx 警告 = 失败
  
  public-api-check:
    # 跑 PublicApiAnalyzers，确保 Shipped.txt + Unshipped.txt 一致
  
  pack-alpha:
    if: github.ref == 'refs/heads/main' && success()
    # 发 -alpha 包到 GitHub Packages
```

## 4. 质量承诺（对外）

Skywalker v2.0 stable 发布时承诺：

> **80%** 的常见场景，第一次就能跑。
>
> **18%** 的边缘场景，编译错误明确指出问题，用户改一下就 OK。
>
> **2%** 的真·未知场景，报 issue → **1 周内 patch 版本**修复。

**不承诺"已经覆盖所有场景"，承诺"遇到不支持的场景给你明确的编译错误"。**

## 5. 流程规范

### 5.1 新增 SG 流程

1. 在 v2.0 Epic Issue 下开子 issue，描述目标
2. Sprint 0 基础设施先就位（一次性）
3. 创建分支 `feat/sg-<功能>`
4. 实现 generator + 测试 + sample
5. PR 时勾选 §9 检查清单
6. Review + downstream 验证
7. 合并 → 自动发 -alpha
8. 收集 1-2 周种子用户反馈
9. 进入下一个 Sprint

### 5.2 Bug 响应 SLA

| 严重级 | 响应 | 修复 |
|---|---|---|
| P0（生成代码导致编译失败 / 运行炸） | 24h | 1 周内 patch |
| P1（功能性缺失，无 workaround） | 3 天 | 2 周内 patch |
| P2（功能性缺失，有 workaround） | 1 周 | 下个 minor |
| P3（优化建议、文档问题） | 2 周 | 视情况 |

### 5.3 浸泡期违规处理

如果出现以下情况，**重置浸泡计时**：

- preview/rc 期间发现 P0 → 修复后浸泡时间从 0 重新计算
- preview/rc 期间合并大型功能改动 → 浸泡时间从 0 重新计算

## 6. 维护者纪律

- **永远不要承诺**"已经覆盖所有场景"。
- **永远不要**为了赶版本跳过浸泡期。
- **永远不要**接受没有 snapshot 测试的 SG PR。
- **永远不要**默默 `return` 不报诊断。
- **永远不要**让 `MakeGenericType` / `Reflection.Emit` 出现在 v2.0 热路径。
