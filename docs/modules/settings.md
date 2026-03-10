# 设置管理模块

本文档详细介绍 Skywalker 框架的设置管理模块。

---

## 目录

1. [模块概述](#模块概述)
2. [Skywalker.Settings.Abstractions](#skywalkersettingsabstractions)
3. [Skywalker.Settings.Domain](#skywalkersettingsdomain)
4. [Skywalker.Settings.EntityFrameworkCore](#skywalkersettingsentityframeworkcore)
5. [Skywalker.Settings.Redis](#skywalkersettingsredis)

---

## 模块概述

| 模块 | NuGet 包 | 说明 |
|------|----------|------|
| Skywalker.Settings.Abstractions | `Skywalker.Settings.Abstractions` | 设置抽象接口 |
| Skywalker.Settings.Domain | `Skywalker.Settings.Domain` | 设置领域层 |
| Skywalker.Settings.EntityFrameworkCore | `Skywalker.Settings.EntityFrameworkCore` | EF Core 存储 |
| Skywalker.Settings.Redis | `Skywalker.Settings.Redis` | Redis 存储 |

### 依赖关系

```
Skywalker.Settings.Domain
└── Skywalker.Settings.Abstractions

Skywalker.Settings.EntityFrameworkCore
├── Skywalker.Settings.Domain
└── Skywalker.Ddd.EntityFrameworkCore

Skywalker.Settings.Redis
├── Skywalker.Settings.Abstractions
└── Microsoft.Extensions.Caching.StackExchangeRedis
```

### 设置值提供者优先级

```
1. User (U)          - 用户级别设置
2. Tenant (T)        - 租户级别设置
3. Global (G)        - 全局设置
4. Configuration (C) - 配置文件设置
5. Default (D)       - 默认值
```

---

## Skywalker.Settings.Abstractions

### 简介

设置抽象模块，定义设置系统的核心接口。

### 安装

```bash
dotnet add package Skywalker.Settings.Abstractions
```

### 核心类型

#### ISettingProvider - 设置提供者接口

```csharp
namespace Skywalker.Settings.Abstractions;

public interface ISettingProvider
{
    /// <summary>
    /// 获取设置值（不存在时抛出异常）
    /// </summary>
    Task<string> GetAsync(string name);
    
    /// <summary>
    /// 获取设置值（不存在时返回 null）
    /// </summary>
    Task<string?> GetOrNullAsync(string name);
    
    /// <summary>
    /// 批量获取设置值
    /// </summary>
    Task<List<SettingValue>> GetAllAsync(string[] names);
    
    /// <summary>
    /// 获取所有设置
    /// </summary>
    Task<List<SettingValue>> GetAllAsync();
}
```

#### ISettingStore - 设置存储接口

```csharp
namespace Skywalker.Settings.Abstractions;

public interface ISettingStore
{
    /// <summary>
    /// 获取设置值
    /// </summary>
    /// <param name="name">设置名称</param>
    /// <param name="providerName">提供者名称（G/T/U）</param>
    /// <param name="providerKey">提供者键（如用户ID、租户ID）</param>
    Task<string?> GetOrNullAsync(string name, string providerName, string? providerKey);
    
    /// <summary>
    /// 批量获取设置值
    /// </summary>
    Task<List<SettingValue>> GetAllAsync(string[] names, string providerName, string? providerKey);
}
```

#### ISettingValueProvider - 设置值提供者接口

```csharp
namespace Skywalker.Settings.Abstractions;

public interface ISettingValueProvider
{
    /// <summary>
    /// 提供者名称
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// 获取设置值
    /// </summary>
    Task<string?> GetOrNullAsync(SettingDefinition setting);
    
    /// <summary>
    /// 批量获取设置值
    /// </summary>
    Task<List<SettingValue>> GetAllAsync(SettingDefinition[] settings);
}
```

#### SettingDefinition - 设置定义

```csharp
namespace Skywalker.Settings;

public class SettingDefinition
{
    /// <summary>
    /// 设置唯一名称
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// 显示名称
    /// </summary>
    public string DisplayName { get; set; }
    
    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// 默认值
    /// </summary>
    public string? DefaultValue { get; set; }
    
    /// <summary>
    /// 是否对客户端可见
    /// </summary>
    public bool IsVisibleToClients { get; set; }
    
    /// <summary>
    /// 是否继承（从父级获取）
    /// </summary>
    public bool IsInherited { get; set; }
    
    /// <summary>
    /// 是否加密存储
    /// </summary>
    public bool IsEncrypted { get; set; }
    
    public SettingDefinition(
        string name,
        string? defaultValue = null,
        string? displayName = null,
        string? description = null,
        bool isVisibleToClients = false,
        bool isInherited = true,
        bool isEncrypted = false);
}
```

#### SettingValue - 设置值

```csharp
namespace Skywalker.Settings;

[Serializable]
public class SettingValue : NameValue
{
    public SettingValue(string name, string? value) : base(name, value) { }
}
```

