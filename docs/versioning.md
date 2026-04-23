# Skywalker 版本策略

> 最后更新：2026-04-23

本文档规定 Skywalker 的版本号来源、发布流程、以及 v1.x / v2.0 双线并存的演进路径。**这是唯一权威文档**，CI 工作流、NuGet 包、发布公告都以此为准。

---

## 核心原则

1. **tag 是事实源**。git tag 决定产出的 NuGet 版本号，不存在任何"手工维护的版本字段"。
2. **分支 push = 预发布**。推到 `main` / `release/2.0` 会自动发 `-alpha.N` / `-preview.N` 包，供下游提前集成测试，但**不**承诺稳定性。
3. **tag 推送 = 承诺**。打 `v*` tag 即对外承诺这一版本的 API 稳定性边界（见下表）。

---

## 版本号派生规则

由 [MinVer](https://github.com/adamralph/minver) 在 `dotnet pack` 时读取 git history 自动推导：

| git 状态 | 产出包版本 | 语义 |
|---|---|---|
| 正好在 `v1.0.0` tag 上 | `1.0.0` | v1.x GA |
| 正好在 `v1.0.1-rc.1` tag 上 | `1.0.1-rc.1` | v1.0.1 的候选发布 |
| 正好在 `v2.0.0-preview.1` tag 上 | `2.0.0-preview.1` | v2.0 首个 preview |
| main，`v1.0.0` 之后 N 个 commit | `1.0.1-alpha.0.N` | v1.x 的日常预览 |
| release/2.0，`v2.0.0-preview.1` 之后 N 个 commit | `2.0.0-preview.1.N` | v2.0 的日常预览 |

配置位置：[Directory.Build.props](../../Directory.Build.props)（`MinVerTagPrefix=v`、`MinVerDefaultPreReleaseIdentifiers=alpha.0`）。

---

## 发布类型与稳定性承诺

| 后缀 | 含义 | 稳定性承诺 | 下游建议 |
|---|---|---|---|
| *（无后缀）* | **GA** | API 稳定，语义化版本承诺 | 生产可用；按需升级 patch/minor |
| `-rc.N` | Release Candidate | **不**再引入新 API，仅修 rc 期间发现的 bug | 生产前集成验证 |
| `-preview.N[.M]` | Preview | API 可能微调；无 breaking 内部变更 | 预研 / staging 环境试用 |
| `-alpha.N[.M]` | Alpha | 任何东西都可能变，仅供日常同步 | **不**生产使用；仅用于团队联调 |
| `-beta.N`（已废弃）| 历史遗物 | v1.0.0 之前的旧版本用过 | 迁移到 `1.0.0` 或更高 |

---

## 版本生命周期（v1.x / v2.0 双线）

### v1.x — 维护期

| 阶段 | 开始点 | 活动 |
|---|---|---|
| **Active 维护** | `v1.0.0` tag (2026-04-23) | 接受 bug fix 和安全修复，**不再加新功能**；patch 发布 `1.0.1` / `1.0.2` / ... |
| **LTS** | v2.0 GA 后 | 仅安全修复和 P0/P1 bug fix；持续 6 个月 |
| **EOL** | LTS 结束 | 不再发布任何更新 |

### v2.0 — 开发期

| 阶段 | 开始点 | 活动 |
|---|---|---|
| **Preview** | `v2.0.0-preview.1` tag (2026-04-23) | Source Generator 重写在进行；API 可能微调 |
| **RC** | 所有 Sprint 关闭 + AOT 零警告 | 打 `v2.0.0-rc.1`；仅修 bug |
| **GA** | RC 浸泡 ≥ 2 周无 P0/P1 | 打 `v2.0.0`；公开发布 |

GA 的硬门禁详见 [docs/architecture/v2.0-roadmap.md](./architecture/v2.0-roadmap.md)。

---

## 分支与发布映射

```
                        (手动打 tag)
                    ┌─► v1.0.1-rc.1  ────► 1.0.1-rc.1
main ──── push ────┤
                    └─► 日常 push ────────► 1.0.X-alpha.0.N

                        (手动打 tag)
                    ┌─► v2.0.0-preview.2 ─► 2.0.0-preview.2
release/2.0 ──push─┤
                    └─► 日常 push ────────► 2.0.0-preview.1.N
                                              （N 基于 v2.0.0-preview.1 之后的 commit 数）
```

### forward-merge

`main` 的每次 push 由 [`forward-merge.yml`](../../.github/workflows/forward-merge.yml) 自动合并到 `release/2.0`。bug fix 双享受；新功能**不**走这条路（新功能应直接 target `release/2.0`）。

冲突处理见 [CONTRIBUTING.md 分支策略](../../CONTRIBUTING.md#-分支策略)。

---

## 打 tag 的操作

### v1.x patch 发布（例：1.0.1）

```bash
# 1. 确认 main 健康 (CI 绿、没有待解决 P0/P1)
# 2. 从 main HEAD 打 tag
git checkout main && git pull --ff-only
git tag -a v1.0.1-rc.1 -m "v1.0.1-rc.1 — candidate"
git push origin v1.0.1-rc.1
# → CI 发布 Skywalker.*.1.0.1-rc.1.nupkg

# 3. RC 浸泡 ≥ 5 工作日，无 P0/P1
git tag -a v1.0.1 -m "v1.0.1"
git push origin v1.0.1
# → CI 发布 Skywalker.*.1.0.1.nupkg (GA)
```

### v2.0 preview 迭代（例：preview.2）

```bash
git checkout release/2.0 && git pull --ff-only
git tag -a v2.0.0-preview.2 -m "v2.0.0-preview.2"
git push origin v2.0.0-preview.2
# → CI 发布 Skywalker.*.2.0.0-preview.2.nupkg
# 后续 release/2.0 的 push 会自动产出 2.0.0-preview.2.N
```

### v2.0 GA（例：2.0.0）

```bash
# 经过 preview.N → rc.N → rc 浸泡的标准流程
git checkout release/2.0 && git pull --ff-only
git tag -a v2.0.0 -m "v2.0.0 — first stable release of the SG rewrite"
git push origin v2.0.0

# GA 后 release/2.0 合并回 main，release/2.0 分支保留仅做历史参照
git checkout main
git merge release/2.0
git push origin main
```

---

## FAQ

### 为什么 `v1.0.0` tag 是 2026-04-23 才打？

在此之前的发布都是 `1.0.0-beta.N`（N = GitHub Actions run_number），没有承诺 SemVer 稳定性。
打 `v1.0.0` 是把当前 main HEAD 作为**诚实快照** —— "这就是现在的状态，bug 不完美但可用，承诺从此按 SemVer 迭代 patch"。
详见 [PR #213 commit message](https://github.com/dengxuan/Skywalker/pull/213)。

### 我要发布 1.0.1，能直接从 main HEAD 打 tag 吗？

可以，但**推荐先走 rc**：先打 `v1.0.1-rc.1` 跑 5 工作日；仅在 `rc.1` 无任何 P0/P1 bug 时再打 `v1.0.1`。
跳过 rc 的前提是：patch 内容极简（单一 bug fix、diff ≤ 50 行），且 CI 全绿、手动回归通过。

### 2.0.0-preview.1 之后想再发一个 preview，选 preview.2 还是让 MinVer 自动加 height？

- **选 preview.2**：当你想明确标记 "这是一个稳定的 preview 里程碑"（内部 staging 可以 pin 它）。
- **让 MinVer 自动加**：日常开发阶段，`2.0.0-preview.1.N` 就够了，每个 commit 都能被下游试用。

一般节奏：每 2-4 周打一个 `preview.M`，期间靠 `preview.(M-1).N` 持续迭代。

### 哪些文件**不应**被手工改动？

- `eng/Versions.props` 的项目版本字段（MajorVersion / MinorVersion / PatchVersion 等）—— 已删除，不要恢复。
- 不要在 CI 里 `sed` 或 `git commit` 版本号。一切走 MinVer。

---

## 相关

- [CONTRIBUTING.md § 分支策略](../../CONTRIBUTING.md#-分支策略)
- [docs/migration/v1-to-v2.md](./migration/v1-to-v2.md) — v1.x → v2.0 迁移手册
- [docs/architecture/v2.0-roadmap.md](./architecture/v2.0-roadmap.md) — v2.0 路线图
- [Directory.Build.props](../../Directory.Build.props) — MinVer 配置
- [.github/workflows/nupkg-publish.yml](../../.github/workflows/nupkg-publish.yml) — 发布 workflow
