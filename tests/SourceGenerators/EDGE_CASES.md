# Skywalker Source Generator 边界场景清单

> 状态：**活清单 (Living Document)**　|　适用：v2.0+　|　配套规范：[`source-generators-quality.md` §1.2 Layer 2](../../docs/architecture/source-generators-quality.md#layer-2-edge-case-测试矩阵)

## 这份清单做什么

Skywalker 任何 Source Generator 在合并到 main 前，**必须**对照本清单逐项确认：

- ✅ **已覆盖**：snapshot 测试或单元测试明确验证。
- 🚧 **明确不支持**：报 `SKYxxxx` 诊断，并列出对应诊断码。
- ⏭️ **不适用**：场景与该 SG 无关（例如 EF Repo SG 不需要考虑 `ref struct` 参数）。

PR 描述里列出对照结果。**不允许出现"未明确状态"**——边界场景悄悄不工作是 SG 最常见的事故源。

## 如何使用

1. 复制本清单到 PR 描述
2. 把每个 case 的状态填成 `✅` / `🚧 SKYxxxx` / `⏭️`
3. `🚧` 必须给出具体诊断码（通过 #190 [`docs/diagnostics/`](../../docs/diagnostics/README.md) 检索）
4. ⏭️ 必须给出 1 行理由

## 类型形态

- [ ] 普通 `class`
- [ ] `partial class`（多文件分布）
- [ ] `sealed class`
- [ ] `abstract class`（应跳过 + 报诊断）
- [ ] `static class`（应跳过 + 报诊断）
- [ ] `record`
- [ ] `record struct`
- [ ] 开放泛型 `class<T>`
- [ ] 闭合泛型 `class<int>`
- [ ] 多层嵌套类（`Outer.Inner.Target`）
- [ ] file-scoped 类（C# 11 `file class`）

## 可见性

- [ ] `public`
- [ ] `internal`
- [ ] `private` 嵌套
- [ ] `protected` / `protected internal` 嵌套（视场景）
- [ ] file-scoped（仅本文件可见）
- [ ] `InternalsVisibleTo` 暴露的 internal

## 命名空间

- [ ] file-scoped namespace（`namespace Foo;`）
- [ ] block-scoped namespace（`namespace Foo { ... }`）
- [ ] 全局命名空间（无 `namespace` 声明）
- [ ] `using` alias（`using OrderRepo = Foo.OrderRepository;`）
- [ ] `global using`
- [ ] 命名空间别名 `extern alias`（应跳过 + 报诊断）
- [ ] 同一命名空间在多个 partial 文件
- [ ] 命名空间含特殊字符（`@class`、`@namespace`）

## 接口实现

- [ ] 单接口
- [ ] 多接口实现
- [ ] 接口继承链（`ISpecific : IBase`）
- [ ] 跨程序集接口
- [ ] 显式接口实现（`Task IFoo.GetAsync()`）
- [ ] 默认接口方法（C# 8+ DIM）
- [ ] 接口的泛型约束（`where T : IFoo`）
- [ ] 接口含 `static abstract`（C# 11+，应视情况报诊断）

## 继承关系

- [ ] 无基类
- [ ] 同程序集基类
- [ ] 跨程序集基类
- [ ] 多层继承链
- [ ] 基类 abstract（含未实现 abstract 成员）
- [ ] 基类 sealed（无法继承——SG 应能识别）

## 方法签名

### 返回值

- [ ] `void`
- [ ] sync 返回（`int`、`Order`）
- [ ] `Task`
- [ ] `Task<T>`
- [ ] `ValueTask` / `ValueTask<T>`
- [ ] `IAsyncEnumerable<T>`
- [ ] `ref T` 返回
- [ ] `ref readonly T` 返回

### 参数

- [ ] 无参数
- [ ] 普通值类型 / 引用类型
- [ ] `in` / `ref` / `out`
- [ ] `params` 数组
- [ ] 默认值参数（`int x = 10`）
- [ ] `ref struct` 参数（`Span<T>` 等，多数 SG 应报警告或跳过）
- [ ] nullable 参考类型（`string?`）
- [ ] nullable 值类型（`int?`）
- [ ] 可空泛型（`T?` where T : class / struct）
- [ ] `dynamic`（应跳过或报诊断）

### 修饰

- [ ] `static` 方法
- [ ] `virtual` / `override`
- [ ] `abstract`
- [ ] `sealed override`
- [ ] `async` / 同步混合
- [ ] 索引器（`this[int i]`）
- [ ] operator（`operator +`）
- [ ] 显式 / 隐式转换运算符
- [ ] 局部函数（不应被 SG 拾取）

### 泛型方法

- [ ] 无约束（`<T>`）
- [ ] `where T : new()`
- [ ] `where T : class`
- [ ] `where T : struct`
- [ ] `where T : unmanaged`
- [ ] `where T : notnull`
- [ ] `where T : <BaseType>`
- [ ] `where T : <Interface>`
- [ ] 多约束链（`where T : class, IFoo, new()`）

## 程序集 / 模块

- [ ] 同程序集
- [ ] 跨程序集继承
- [ ] 跨程序集接口实现
- [ ] `InternalsVisibleTo` 暴露的类型
- [ ] 同一类型在多个程序集都被处理（应去重）
- [ ] PackageReference vs ProjectReference（行为应一致）
- [ ] AOT publish（无 IL2xxx / IL3xxx 警告）

## 增量与缓存

- [ ] 不修改源 → 第二次运行 100% 缓存命中
- [ ] 仅修改 method body → 不应触发重新生成（如果生成只依赖签名）
- [ ] 仅修改 trivia / 注释 → 不应触发重新生成
- [ ] 添加无关类 → 已处理类的生成结果不变
- [ ] 大量文件下生成耗时（≥ 1000 文件 < 5s）

## 编译错误鲁棒性

- [ ] 源代码本身有 syntax error → SG 不崩溃
- [ ] 标注的类引用了不存在的类型 → SG 不崩溃，报诊断
- [ ] `partial` 不一致（一处 partial 一处非 partial） → SG 报诊断
- [ ] 同一类被多个 SG 处理 → 生成的成员不冲突（命名隔离）

## 标注 / Attribute

- [ ] 同一类标注多个相关 attribute
- [ ] attribute 用 `nameof()` / `typeof()` 参数
- [ ] attribute 用 const 表达式
- [ ] attribute 含 `null` 参数
- [ ] attribute 上声明在 partial 类的不同分部
- [ ] attribute 来自跨程序集
- [ ] 命名冲突（多个不同 namespace 的同名 attribute）

## 命名冲突

- [ ] 多个标注同名类（不同 namespace）
- [ ] 用户类与生成类同名（应优先用户的，或报诊断）
- [ ] 用户已实现 partial method → SG 不应重复实现
- [ ] 生成代码使用的辅助类型与用户类型同名（应做命名隔离）

## 用户工程配置

- [ ] `<Nullable>enable</Nullable>` vs `disable`
- [ ] `<LangVersion>` 8 / 9 / 10 / 11 / 12 / latest
- [ ] `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`（生成代码不能有 warning）
- [ ] `EmitCompilerGeneratedFiles=true`（用户能在 `obj/Generated/` 看到生成代码）
- [ ] `<AnalysisMode>` All / Default / None
- [ ] 用户在 `.editorconfig` 关闭某些 IDE0xxx → 生成代码不应触发那些规则

## 工具链 / IDE

- [ ] Visual Studio 实时增量更新（保存后立即反映）
- [ ] Rider 实时增量更新
- [ ] VS Code + C# Dev Kit
- [ ] `dotnet build` 命令行
- [ ] `dotnet test` 命令行
- [ ] CI（无 IDE 缓存）

## 模板

下面是 PR 描述里粘贴的清单模板：

```markdown
### EDGE_CASES 对照（针对 <Generator 名>）

#### 类型形态
- ✅ 普通 class
- ✅ partial class
- 🚧 abstract class — SKY9002
- 🚧 static class — SKY9003
- ⏭️ record struct — 该 SG 仅处理 class
- ...

#### 可见性
...
```

## 维护

- 加新场景：直接在对应类别下加一行；不要删旧条目（保留历史）。
- 调整分类：跨类别移动需在 PR 描述里说明原因。
- 该清单与 [`source-generators-quality.md`](../../docs/architecture/source-generators-quality.md) §1.2 联动；规范变更时同步更新。
