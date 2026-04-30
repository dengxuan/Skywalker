# Skywalker 研发与发版治理规范

> 最后更新：2026-04-30
> 版本号事实源见 [versioning.md](versioning.md)。本文定义研发节奏、bugfix 路径、main 分支职责和 daily build。

## 1. 目标

Skywalker 的发布治理遵守四个原则：

1. `main` 必须始终代表当前稳定线的最新可发布状态。
2. 新功能进入下一代开发线，稳定线只接收 bug fix、安全修复和仓库治理改动。
3. 每个合并到长期分支的提交都必须可构建、可测试、可打包。
4. 合并到长期分支后自动发布 GitHub Packages 滚动包，供下游及时验证；正式版本只由 tag 发布。

## 2. 分支职责

| 分支 | 当前职责 | 合并规则 | 发布通道 |
|---|---|---|---|
| `main` | v1.x 最新稳定线 | 只接受 v1.x bugfix、安全修复、仓库治理；必须 CI 绿 | `1.0.X-alpha.0.N` GitHub Packages 滚动包；`v1.x.y[-rc.n]` tag 发布正式包 |
| `release/2.0` | v2.0 开发集成线 | 接受 v2.0 feature/refactor/docs；接收 main forward-merge | `2.0.0-preview.N.M` GitHub Packages 滚动包；`v2.0.0-preview.N` / `v2.0.0-rc.N` / `v2.0.0` tag 发布正式包 |
| `feature/*` | 单一功能或 issue | 从目标长期分支切出，PR 合并回目标长期分支 | 不发布 |
| `fix/*` | 单一 bugfix | 影响 v1.x 用户则 target `main`；只影响 v2.0 则 target `release/2.0` | 不发布 |
| `integration/*` | 短期专题集成分支 | 仅用于一个 Sprint 或一组强相关 PR 的临时集成，结束后合回 `release/2.0` 并删除 | 不发布 |
| `docs/*` / `chore/*` | 文档、CI、治理 | 依据影响范围 target `main` 或 `release/2.0` | 不发布 |

### 不设长期 dev 分支

Skywalker 不维护长期 `dev` 分支。`release/2.0` 已承担 v2.0 开发集成职责，同时带有明确版本目标和发布节奏；再增加长期 `dev` 会让 `main`、`release/2.0`、`dev` 三条长期线同时存在，增加 forward-merge、daily build、版本号和发布判断的复杂度。

需要开发缓冲区时，使用短期 `integration/*` 分支：

1. 从 `release/2.0` 切出，例如 `integration/sg-sprint-1`。
2. 只接收同一 Sprint 或强相关专题的 PR。
3. 每天跟进 `release/2.0`，保持可构建、可测试。
4. Sprint 或专题完成后合回 `release/2.0`，随后删除该分支。
5. `integration/*` 不打 tag、不发包、不作为 daily build 长期矩阵目标。

### v2.0 GA 后

当 `v2.0.0` GA tag 通过后：

1. 将 `release/2.0` fast-forward 或 merge 回 `main`。
2. `main` 成为 v2.x 最新稳定线。
3. v1.x 进入 LTS 分支（如 `release/1.x`，仅安全修复和 P0/P1）。
4. 后续所有常规开发以 `main` 为最新版本主线；新的大版本再开 `release/3.0`。

这保证 `main` 在任意时刻都有明确含义：过渡期是最新 v1 稳定线，v2 GA 后是最新稳定大版本。

## 3. 日常研发流程

1. 先判断目标分支：v1.x 用户可见 bugfix 走 `main`；v2.0 SG/新功能走 `release/2.0`；短期专题集成才走 `integration/*`。
2. 从目标分支新建短分支：`feature/issue-123-short-name`、`fix/issue-456-short-name`。
3. 一个分支只解决一个 issue 或一组强相关改动；跨 issue 的基础设施、docs、CI 分开提交。
4. PR 必须说明目标分支、关联 issue、风险、测试结果、是否影响 public API。
5. 用户可见的修复、功能、破坏性变更必须在同一个 PR 中更新 `CHANGELOG.md` 的 `[Unreleased]`；缺失时不得合并。
6. 仅纯内部重构、测试、构建脚本或无用户可见变化的 PR 可以不改 `[Unreleased]`，但 PR 描述必须写明 `No changelog needed` 和原因。
7. `changelog-check.yml` 会在 PR 中检查用户可见改动是否同步修改 `CHANGELOG.md`；未满足时阻塞合并。
8. 合并前至少满足：
   - `dotnet build Skywalker.sln --configuration Release --no-restore -m:1`
   - `dotnet test Skywalker.sln --configuration Release --no-build -m:1`
   - 涉及 SG/Analyzer 时跑对应 SG quality gate。
9. 合并后由 CI 自动 forward-merge `main` 到 `release/2.0`；冲突由维护者手工解决。

## 4. 滚动验证包与 Unreleased

Skywalker 采用“长期分支滚动包 + Unreleased 浸泡 + tag 转正式版本”的节奏，和正式 NuGet 版本解耦：

1. PR 合并到 `main` 或 `release/2.0` 后，[`nupkg-publish.yml`](../.github/workflows/nupkg-publish.yml) 自动打包并发布到 GitHub Packages。
2. 这些包是滚动验证包，只用于下游及时验证，不代表稳定承诺。
3. `CHANGELOG.md` 的 `[Unreleased]` 记录当前滚动包已经包含、但尚未转成正式版本的变更；每个用户可见 PR 都必须自带自己的 `[Unreleased]` 条目，避免合并后遗漏。
4. 滚动包经过 1-2 周浸泡且无 P0/P1 后，将 `[Unreleased]` 内容移动到具体版本段，例如 `[1.0.1]`、`[2.0.0-preview.2]` 或 `[2.0.0-rc.1]`。
5. 移动后立即创建新的空 `[Unreleased]`，继续记录下一轮修复和功能。
6. 正式交付只通过 `v*` tag 发布；不直接把滚动包当作正式版本宣布。

下游验证流程：

1. 下游先配置 GitHub Packages 源。
2. 维护者合并修复 PR 到目标长期分支。
3. CI 发布滚动包，例如 `1.0.1-alpha.0.N` 或 `2.0.0-preview.1.N`。
4. 下游把依赖切到该滚动版本验证。
5. 下游确认修复后，维护者保留该变更在 `[Unreleased]` 中继续浸泡。
6. 到达发布窗口后，维护者把 `[Unreleased]` 固化为正式版本段并打 tag。

这意味着 bugfix 是否修好，不靠本地猜测，也不靠手工传包；以长期分支滚动包作为下游验证对象。

## 5. Bugfix 路径

| 严重级 | 定义 | 目标分支 | 发布策略 |
|---|---|---|---|
| P0 | 数据损坏、安全漏洞、应用无法启动、编译全面失败 | `main`，并 forward-merge 到 `release/2.0` | 尽快发 patch；可跳过 rc 但必须记录原因 |
| P1 | 核心功能不可用且无合理 workaround | `main` 或 `release/2.0`，取决于受影响版本 | 推荐 `-rc.1` 浸泡 1-5 个工作日 |
| P2 | 功能缺陷但有 workaround | 受影响版本线 | 进入下个 patch/preview |
| P3 | 文档、易用性、低风险修补 | 对应分支 | 随日常版本发布 |

Bugfix PR 必须包含：

- 最小复现或测试用例。
- 修复说明和风险评估。
- `CHANGELOG.md` 的 `[Unreleased]` 条目；如果无需记录，必须说明原因。
- 合并后供下游验证的滚动包版本号。
- 如果影响 v1.x 和 v2.0，说明 forward-merge 是否可能冲突。
- 如果修改 public API，更新对应 `PublicAPI.Unshipped.txt` 并在 PR 描述中标注。

## 6. 发版流程

### v1.x patch

1. 确认 `main` CI 绿，滚动包已由下游验证，daily build 最近一次成功。
2. 将 `CHANGELOG.md` 的 `[Unreleased]` 内容移动到 `[v1.x.y-rc.1]` 或 `[v1.x.y]` 段，并创建新的空 `[Unreleased]`。
3. 打 `v1.x.y-rc.1` tag 并发布 rc。
4. 浸泡至少 5 个工作日；P0 极简修复可由维护者记录原因后跳过 rc。
5. 打 `v1.x.y` tag 发布 GA；如 rc 阶段又有修复，先回到 `[Unreleased]` 记录并重新浸泡。

### v2.0 preview / rc / GA

1. Sprint 完成且 `release/2.0` 滚动包已被下游验证后，将 `[Unreleased]` 内容移动到 `[v2.0.0-preview.N]`。
2. 从 `release/2.0` 打 `v2.0.0-preview.N`；preview 至少浸泡 2 周，期间 P0/P1 会重置该阶段浸泡时间。
3. 所有 Sprint 完成、AOT 零警告、Public API 锁定、迁移文档完整后打 `v2.0.0-rc.1`。
4. rc 至少浸泡 2 周且无 P0/P1 后打 `v2.0.0`。
5. GA 后将 `release/2.0` 合入 `main`，让 `main` 指向最新稳定大版本。

## 7. 如何保证 main 指向最新稳定版本

- 保护 `main`：禁止直接 push，必须 PR，必须通过 required checks。
- 所有 GA tag 从 `main` 当前 HEAD 或即将合入 `main` 的 release 分支 HEAD 创建。
- v2.0 GA 前，`main` 是最新 v1 稳定线；v2.0 GA 后，`release/2.0` 必须合回 `main`。
- 每次 GA 后检查：`git merge-base --is-ancestor <ga-tag> main` 必须成功。
- `CHANGELOG.md`、`docs/versioning.md`、GitHub Release notes 必须指向同一个 tag 和版本号。

## 8. Daily build

Daily build 的目的不是给下游发验证包，而是每天验证长期分支是否仍能 restore、build、test、pack。下游验证使用合并后自动发布到 GitHub Packages 的滚动包。

Daily build 每天只对长期分支 `main` 和 `release/2.0` 执行：

1. checkout 完整历史和 tags，保证 MinVer 可计算版本。
2. restore / Release build / Release test。
3. `dotnet pack` 产出 `.nupkg` 和 `.snupkg`。
4. 上传 artifacts，保留 14 天。
5. 不推送到 NuGet，也不推送到 GitHub Packages。

Daily build 失败处理：

- `main` 失败：当天必须优先修复或 revert，不能继续发 patch tag。
- `release/2.0` 失败：阻塞 preview/rc tag；如果由 main forward-merge 引入，先修冲突或开 rollback PR。
- 连续 2 天失败：开 P1 issue，指定 owner 和修复截止时间。

## 9. 发布前检查清单

- [ ] 目标分支 CI 绿。
- [ ] 最近一次 daily build 绿。
- [ ] GitHub Packages 滚动包已发布，并至少有一个下游验证通过。
- [ ] `CHANGELOG.md` 已将 `[Unreleased]` 内容移动到本次版本段，并创建新的空 `[Unreleased]`。
- [ ] Public API baseline 已更新，`PublicAPI.Unshipped.txt` 状态符合发布阶段。
- [ ] NuGet 包可从 GitHub Packages 滚动版本安装验证。
- [ ] v2.0 发布满足 [v2.0 roadmap](architecture/v2.0-roadmap.md) 的质量门禁。
- [ ] tag 名符合 [versioning.md](versioning.md)。