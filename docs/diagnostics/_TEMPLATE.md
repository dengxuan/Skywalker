# SKYxxxx: 简短标题（祈使句或现在时陈述句）

| 项 | 值 |
|---|---|
| 严重性 | Error / Warning / Info |
| 引入版本 | 2.0.0 |
| 类别 | `Skywalker.SourceGenerators` |
| 关联 SG | `Skywalker.Xxx.SourceGenerators` |

## 原因

为什么会触发这个诊断。1-3 句话讲清楚 SG 的硬约束以及它为什么必须存在。

## 错误示例

```csharp
// 触发 SKYxxxx 的代码
public class OrderAppService  // ← 缺少 partial
{
    // ...
}
```

> SG 编译期诊断：
> ```
> error SKYxxxx: Class 'OrderAppService' marked with [ApplicationService] must be declared as partial
> ```

## 正确示例

```csharp
// 修复后
public partial class OrderAppService
{
    // ...
}
```

## 自动修复

如果该诊断有 Roslyn Code Fix（Sprint 4 stretch goal），在此说明：

- ✅ 一键修复：把 `class` 改为 `partial class`
- ❌ 暂无自动修复

## 相关链接

- [SG 设计规范 §<相关章节>](../architecture/source-generators-spec.md#<anchor>)
- [EDGE_CASES](../../tests/SourceGenerators/EDGE_CASES.md) 中的 `<场景>` 条目

## 修订历史

| 版本 | 变更 |
|---|---|
| 2.0.0 | 引入 |
