# Skywalker 诊断码索引

> 状态：**活索引 (Living Index)**　|　适用：v2.0+　|　规范：[`source-generators-spec.md` §5](../architecture/source-generators-spec.md#5-诊断-diagnostics-规范)

Skywalker Source Generator 遇到不支持的场景时**绝不静默跳过**，必须报 `SKYxxxx` 诊断。本页汇总所有诊断码，每个码都有详细文档页。

## 段位规划

| 段位 | 用途 |
|---|---|
| `SKY1xxx` | DI / 服务注册相关 |
| `SKY2xxx` | DynamicProxy / 拦截器相关 |
| `SKY3xxx` | EF Repository 相关 |
| `SKY4xxx` | EventBus 相关 |
| `SKY5xxx` | Permission / Localization / Settings 相关 |
| `SKY9xxx` | 通用约束（必须 partial、必须 public 等） |

> 段位由 [`source-generators-spec.md` §1.3](../architecture/source-generators-spec.md#13-诊断码段位) 定义；新增段位需先更新规范文档。

## 诊断列表

> 按 ID 升序排列。新增诊断必须在此处登记，并配套 [`SKYxxxx.md`](_TEMPLATE.md) 文档页。

### SKY9xxx — 通用

| ID | 标题 | 严重性 | 引入版本 |
|---|---|---|---|
| [SKY9001](SKY9001.md) | Service class must be partial | Error | 2.0.0 |

### SKY1xxx — DI / 服务注册

_暂无。Sprint 3 引入。_

### SKY2xxx — DynamicProxy / 拦截器

_暂无。Sprint 2 引入。_

### SKY3xxx — EF Repository

_暂无。Sprint 1 引入。_

### SKY4xxx — EventBus

_暂无。规划中。_

### SKY5xxx — Permission / Localization / Settings

_暂无。Sprint 4 stretch goal。_

## 编写新诊断页

1. 复制 [`_TEMPLATE.md`](_TEMPLATE.md) 为 `SKYxxxx.md`，文件名是诊断 ID 大写。
2. 在本页"诊断列表"对应段位下登记（保持按 ID 排序）。
3. 在 SG 代码里 `helpLinkUri` 设为 `https://github.com/dengxuan/Skywalker/blob/main/docs/diagnostics/SKYxxxx.md`。
4. 在 SG 测试项目里加 `*_ShouldReportDiagnostic` 测试。
5. 在 [`tests/SourceGenerators/EDGE_CASES.md`](../../tests/SourceGenerators/EDGE_CASES.md) 对应场景登记 🚧 SKYxxxx。

## 域名与链接

`helpLinkUri` 当前指向 GitHub raw：

```
https://github.com/dengxuan/Skywalker/blob/main/docs/diagnostics/SKYxxxx.md
```

后续如有 `skywalker.dev` 自有域名，再统一切换。链接策略变动会更新本节。
