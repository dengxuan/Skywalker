# 权限管理模块

本文档详细介绍 Skywalker 框架的权限管理模块。

---

## 目录

1. [模块概述](#模块概述)
2. [Skywalker.Permissions.Abstractions](#skywalkerpermissionsabstractions)
3. [Skywalker.Permissions.AuthorizeValidation](#skywalkerpermissionsauthorizevalidation)

---

## 模块概述

| 模块 | NuGet 包 | 说明 |
|------|----------|------|
| Skywalker.Permissions.Abstractions | `Skywalker.Permissions.Abstractions` | 权限抽象接口 |
| Skywalker.Permissions.AuthorizeValidation | `Skywalker.Permissions.AuthorizeValidation` | 远程权限验证 |

### 依赖关系

```
Skywalker.Permissions.AuthorizeValidation
├── Skywalker.Permissions.Abstractions
└── Microsoft.Extensions.Caching.Memory
```

### 权限值提供者

| 提供者 | 名称 | 说明 |
|--------|------|------|
| UserPermissionValueProvider | U | 用户权限 |
| RolePermissionValueProvider | R | 角色权限 |
| ClientPermissionValueProvider | C | 客户端权限 |

---

## Skywalker.Permissions.Abstractions

### 简介

权限抽象模块，定义权限系统的核心接口。

### 安装

```bash
dotnet add package Skywalker.Permissions.Abstractions
```

### 核心类型

#### IPermissionChecker - 权限检查器接口

```csharp
namespace Skywalker.Permissions.Abstractions;

public interface IPermissionChecker
{
    /// <summary>
    /// 检查当前用户是否拥有指定权限
    /// </summary>
    Task<bool> IsGrantedAsync(string name);
    
    /// <summary>
    /// 检查指定主体是否拥有指定权限
    /// </summary>
    Task<bool> IsGrantedAsync(ClaimsPrincipal? claimsPrincipal, string name);
    
    /// <summary>
    /// 批量检查权限
    /// </summary>
    Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names);
    
    /// <summary>
    /// 批量检查指定主体的权限
    /// </summary>
    Task<MultiplePermissionGrantResult> IsGrantedAsync(ClaimsPrincipal? claimsPrincipal, string[] names);
}
```

#### IPermissionDefinitionProvider - 权限定义提供者接口

```csharp
namespace Skywalker.Permissions.Abstractions;

public interface IPermissionDefinitionProvider
{
    /// <summary>
    /// 定义权限
    /// </summary>
    void Define(PermissionDefinitionContext context);
}
```

#### IPermissionValueProvider - 权限值提供者接口

```csharp
namespace Skywalker.Permissions.Abstractions;

public interface IPermissionValueProvider
{
    /// <summary>
    /// 提供者名称
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// 检查权限
    /// </summary>
    Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context);
    
    /// <summary>
    /// 批量检查权限
    /// </summary>
    Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context);
}
```

#### PermissionDefinition - 权限定义

```csharp
namespace Skywalker.Permissions;

public class PermissionDefinition
{
    /// <summary>
    /// 权限唯一名称
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// 显示名称
    /// </summary>
    public string DisplayName { get; }
    
    /// <summary>
    /// 父权限
    /// </summary>
    public PermissionDefinition? Parent { get; set; }
    
    /// <summary>
    /// 子权限列表
    /// </summary>
    public IReadOnlyList<PermissionDefinition> Children { get; }
    
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }
    
    /// <summary>
    /// 允许的提供者列表
    /// </summary>
    public string[] AllowedProviders { get; }
    
    /// <summary>
    /// 自定义属性
    /// </summary>
    public Dictionary<string, object?> Properties { get; }
    
    public PermissionDefinition(
        string name, 
        string displayName, 
        bool isEnabled = true, 
        Dictionary<string, object?>? properties = null, 
        string[]? allowedProviders = null);
    
    /// <summary>
    /// 添加子权限
    /// </summary>
    public virtual PermissionDefinition AddChild(string name, string displayName, bool isEnabled = true);
    
    /// <summary>
    /// 设置属性
    /// </summary>
    public virtual PermissionDefinition WithProperty(string key, object value);
    
    /// <summary>
    /// 设置允许的提供者
    /// </summary>
    public virtual PermissionDefinition WithProviders(params string[] providers);
}
```

#### PermissionGrantResult - 权限授予结果

```csharp
namespace Skywalker.Permissions;

public enum PermissionGrantResult
{
    /// <summary>
    /// 未定义（继续检查下一个提供者）
    /// </summary>
    Undefined,
    
    /// <summary>
    /// 已授予
    /// </summary>
    Granted,
    
    /// <summary>
    /// 已禁止
    /// </summary>
    Prohibited
}
```

