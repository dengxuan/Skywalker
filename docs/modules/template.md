# 模板引擎模块

本文档详细介绍 Skywalker 框架的模板引擎模块。

---

## 目录

1. [模块概述](#模块概述)
2. [Skywalker.Template.Abstractions](#skywalkertemplateabstractions)
3. [Skywalker.Template.Scriban](#skywalkertemplatescriban)
4. [Skywalker.Template.Razor](#skywalkertemplaterazor)

---

## 模块概述

| 模块 | NuGet 包 | 说明 |
|------|----------|------|
| Skywalker.Template.Abstractions | `Skywalker.Template.Abstractions` | 模板抽象接口 |
| Skywalker.Template.Scriban | `Skywalker.Template.Scriban` | Scriban 模板引擎 |
| Skywalker.Template.Razor | `Skywalker.Template.Razor` | Razor 模板引擎 |

### 依赖关系

```
Skywalker.Template.Scriban
├── Skywalker.Template.Abstractions
└── Scriban (5.10.0)

Skywalker.Template.Razor
├── Skywalker.Template.Abstractions
└── RazorLight (2.3.1)
```

---

## Skywalker.Template.Abstractions

### 简介

模板抽象模块，定义模板渲染的核心接口。

### 安装

```bash
dotnet add package Skywalker.Template.Abstractions
```

### 核心类型

#### ITemplateRenderer - 模板渲染器接口

```csharp
namespace Skywalker.Template.Abstractions;

public interface ITemplateRenderer
{
    /// <summary>
    /// 渲染文本模板
    /// </summary>
    /// <param name="templateName">模板名称</param>
    /// <param name="model">模板中使用的模型对象</param>
    /// <param name="cultureName">文化名称，默认使用 CurrentUICulture</param>
    /// <param name="globalContext">全局上下文字典</param>
    Task<string> RenderAsync(
        string templateName, 
        object? model = null, 
        string? cultureName = null, 
        Dictionary<string, object>? globalContext = null);
}
```

#### ITemplateRenderingEngine - 模板渲染引擎接口

```csharp
namespace Skywalker.Template.Abstractions;

public interface ITemplateRenderingEngine
{
    /// <summary>
    /// 引擎名称
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// 渲染模板
    /// </summary>
    Task<string> RenderAsync(
        string templateName, 
        object? model = null, 
        string? cultureName = null, 
        Dictionary<string, object>? globalContext = null);
}
```

#### TemplateDefinition - 模板定义

```csharp
namespace Skywalker.Template;

public class TemplateDefinition
{
    /// <summary>
    /// 模板名称
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// 显示名称
    /// </summary>
    public string DisplayName { get; set; }
    
    /// <summary>
    /// 布局模板名称
    /// </summary>
    public string? Layout { get; set; }
    
    /// <summary>
    /// 渲染引擎名称
    /// </summary>
    public string? RenderEngine { get; set; }
    
    /// <summary>
    /// 本地化资源类型
    /// </summary>
    public Type? LocalizationResource { get; set; }
    
    /// <summary>
    /// 是否内联本地化
    /// </summary>
    public bool IsInlineLocalized { get; set; }
    
    /// <summary>
    /// 默认文化名称
    /// </summary>
    public string? DefaultCultureName { get; set; }
}
```

#### SkywalkerTextTemplatingOptions - 模板选项

```csharp
namespace Skywalker.Template;

public class SkywalkerTextTemplatingOptions
{
    /// <summary>
    /// 模板定义提供者列表
    /// </summary>
    public ITypeList<ITemplateDefinitionProvider> DefinitionProviders { get; }
    
    /// <summary>
    /// 模板内容贡献者列表
    /// </summary>
    public ITypeList<ITemplateContentContributor> ContentContributors { get; }
    
    /// <summary>
    /// 渲染引擎字典
    /// </summary>
    public IDictionary<string, Type> RenderingEngines { get; }
    
    /// <summary>
    /// 默认渲染引擎
    /// </summary>
    public string DefaultRenderingEngine { get; set; }
}
```

### 服务注册

```csharp
services.AddTemplate(options =>
{
    options.DefaultRenderingEngine = "Scriban";
    options.DefinitionProviders.Add<MyTemplateDefinitionProvider>();
});
```

---

## Skywalker.Template.Scriban

### 简介

Scriban 模板引擎实现，提供高性能的文本模板渲染。

### 安装

```bash
dotnet add package Skywalker.Template.Scriban
```

### 核心类型

#### ScribanTemplateRenderingEngine

```csharp
namespace Skywalker.Template.Scriban;

public class ScribanTemplateRenderingEngine : TemplateRenderingEngineBase
{
    public const string EngineName = "Scriban";
    
    public override string Name => EngineName;
    
    public override async Task<string> RenderAsync(
        string templateName,
        object? model = null,
        string? cultureName = null,
        Dictionary<string, object>? globalContext = null);
}
```

### 服务注册

```csharp
services.AddTemplate(options =>
{
    options.DefaultRenderingEngine = ScribanTemplateRenderingEngine.EngineName;
    options.RenderingEngines[ScribanTemplateRenderingEngine.EngineName] = 
        typeof(ScribanTemplateRenderingEngine);
});
```

### Scriban 模板语法

```scriban
{{# 变量输出 }}
Hello, {{ model.name }}!

{{# 条件判断 }}
{{ if model.is_vip }}
  VIP 客户
{{ else }}
  普通客户
{{ end }}

{{# 循环 }}
{{ for item in model.items }}
  - {{ item.name }}: {{ item.price }}
{{ end }}

{{# 函数调用 }}
{{ model.name | string.upcase }}
{{ model.created_at | date.to_string "%Y-%m-%d" }}
```

