# 本地化模块

本文档详细介绍 Skywalker 框架的本地化模块。

---

## 目录

1. [模块概述](#模块概述)
2. [Skywalker.Localization.Abstractions](#skywalkerlocalizationabstractions)
3. [Skywalker.Localization.AspNetCore](#skywalkerlocalizationaspnetcore)
4. [Skywalker.Localization.Json](#skywalkerlocalizationjson)
5. [Skywalker.Localization.EntityFrameworkCore](#skywalkerlocalizationentityframeworkcore)

---

## 模块概述

| 模块 | NuGet 包 | 说明 |
|------|----------|------|
| Skywalker.Localization.Abstractions | `Skywalker.Localization.Abstractions` | 本地化抽象接口 |
| Skywalker.Localization.AspNetCore | `Skywalker.Localization.AspNetCore` | ASP.NET Core 集成 |
| Skywalker.Localization.Json | `Skywalker.Localization.Json` | JSON 文件资源 |
| Skywalker.Localization.EntityFrameworkCore | `Skywalker.Localization.EntityFrameworkCore` | 数据库资源 |

### 依赖关系

```
Skywalker.Localization.AspNetCore
└── Skywalker.Localization.Abstractions

Skywalker.Localization.Json
└── Skywalker.Localization.Abstractions

Skywalker.Localization.EntityFrameworkCore
├── Skywalker.Localization.Abstractions
└── Skywalker.Ddd.EntityFrameworkCore
```

---

## Skywalker.Localization.Abstractions

### 简介

本地化抽象模块，定义多语言支持的核心接口。

### 安装

```bash
dotnet add package Skywalker.Localization.Abstractions
```

### 核心类型

#### IStringLocalizer - 字符串本地化器接口

```csharp
namespace Skywalker.Localization;

public interface IStringLocalizer
{
    /// <summary>
    /// 获取本地化字符串
    /// </summary>
    LocalizedString this[string name] { get; }
    
    /// <summary>
    /// 获取带参数的本地化字符串
    /// </summary>
    LocalizedString this[string name, params object[] arguments] { get; }
    
    /// <summary>
    /// 获取当前文化的所有本地化字符串
    /// </summary>
    IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures = true);
    
    /// <summary>
    /// 获取指定文化的所有本地化字符串
    /// </summary>
    IEnumerable<LocalizedString> GetAllStrings(CultureInfo culture, bool includeParentCultures = true);
}

/// <summary>
/// 泛型本地化器接口
/// </summary>
public interface IStringLocalizer<TResource> : IStringLocalizer
{
}
```

#### IStringLocalizerFactory - 本地化器工厂接口

```csharp
namespace Skywalker.Localization;

public interface IStringLocalizerFactory
{
    /// <summary>
    /// 创建指定资源类型的本地化器
    /// </summary>
    IStringLocalizer Create<TResource>();
    
    /// <summary>
    /// 创建指定资源类型的本地化器
    /// </summary>
    IStringLocalizer Create(Type resourceType);
    
    /// <summary>
    /// 根据名称和位置创建本地化器
    /// </summary>
    IStringLocalizer Create(string baseName, string location);
}
```

#### ILocalizationResource - 本地化资源接口

```csharp
namespace Skywalker.Localization;

public interface ILocalizationResource
{
    /// <summary>
    /// 资源唯一名称
    /// </summary>
    string ResourceName { get; }
    
    /// <summary>
    /// 默认文化名称
    /// </summary>
    string? DefaultCultureName { get; }
    
    /// <summary>
    /// 基础资源类型列表
    /// </summary>
    IReadOnlyList<Type> BaseResourceTypes { get; }
    
    /// <summary>
    /// 资源贡献者列表
    /// </summary>
    IReadOnlyList<ILocalizationResourceContributor> Contributors { get; }
}
```

#### ILocalizationResourceContributor - 资源贡献者接口

```csharp
namespace Skywalker.Localization;

public interface ILocalizationResourceContributor
{
    /// <summary>
    /// 初始化贡献者
    /// </summary>
    void Initialize(LocalizationResourceInitializationContext context);
    
    /// <summary>
    /// 获取本地化字符串
    /// </summary>
    LocalizedString? GetOrNull(string cultureName, string name);
    
    /// <summary>
    /// 填充字典
    /// </summary>
    void Fill(string cultureName, Dictionary<string, LocalizedString> dictionary);
    
    /// <summary>
    /// 获取支持的文化列表
    /// </summary>
    IEnumerable<string> GetSupportedCultures();
}
```

#### LocalizedString - 本地化字符串

```csharp
namespace Skywalker.Localization;

public class LocalizedString
{
    /// <summary>
    /// 资源名称
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// 本地化值
    /// </summary>
    public string Value { get; }
    
    /// <summary>
    /// 资源是否未找到
    /// </summary>
    public bool ResourceNotFound { get; }
    
    /// <summary>
    /// 搜索位置
    /// </summary>
    public string? SearchedLocation { get; }
    
    public LocalizedString(string name, string value, bool resourceNotFound = false, string? searchedLocation = null);
}
```

