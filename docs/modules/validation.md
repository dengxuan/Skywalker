# 数据验证模块

本文档详细介绍 Skywalker 框架的数据验证模块。

---

## 目录

1. [模块概述](#模块概述)
2. [Skywalker.Validation.Abstractions](#skywalkervalidationabstractions)
3. [Skywalker.Validation.FluentValidation](#skywalkervalidationfluentvalidation)

---

## 模块概述

| 模块 | NuGet 包 | 说明 |
|------|----------|------|
| Skywalker.Validation.Abstractions | `Skywalker.Validation.Abstractions` | 验证抽象接口 |
| Skywalker.Validation.FluentValidation | `Skywalker.Validation.FluentValidation` | FluentValidation 集成 |

### 依赖关系

```
Skywalker.Validation.FluentValidation
├── Skywalker.Validation.Abstractions
├── FluentValidation (11.9.0)
└── FluentValidation.DependencyInjectionExtensions (11.9.0)
```

---

## Skywalker.Validation.Abstractions

### 简介

验证抽象模块，定义数据验证的核心接口。

### 安装

```bash
dotnet add package Skywalker.Validation.Abstractions
```

### 核心类型

#### IValidator - 验证器接口

```csharp
namespace Skywalker.Validation;

public interface IValidator
{
    /// <summary>
    /// 验证对象
    /// </summary>
    IValidationResult Validate(object instance);
    
    /// <summary>
    /// 异步验证对象
    /// </summary>
    Task<IValidationResult> ValidateAsync(object instance, CancellationToken cancellationToken = default);
}

/// <summary>
/// 泛型验证器接口
/// </summary>
public interface IValidator<in T> : IValidator where T : class
{
    /// <summary>
    /// 验证对象
    /// </summary>
    IValidationResult Validate(T instance);
    
    /// <summary>
    /// 异步验证对象
    /// </summary>
    Task<IValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default);
}
```

#### IValidationResult - 验证结果接口

```csharp
namespace Skywalker.Validation;

public interface IValidationResult
{
    /// <summary>
    /// 验证是否成功
    /// </summary>
    bool IsValid { get; }
    
    /// <summary>
    /// 验证错误列表
    /// </summary>
    IReadOnlyList<ValidationError> Errors { get; }
}
```

#### ValidationError - 验证错误

```csharp
namespace Skywalker.Validation;

public class ValidationError
{
    /// <summary>
    /// 属性名称
    /// </summary>
    public string PropertyName { get; set; }
    
    /// <summary>
    /// 错误消息
    /// </summary>
    public string ErrorMessage { get; set; }
    
    /// <summary>
    /// 错误代码
    /// </summary>
    public string? ErrorCode { get; set; }
    
    /// <summary>
    /// 尝试的值
    /// </summary>
    public object? AttemptedValue { get; set; }
    
    /// <summary>
    /// 严重程度
    /// </summary>
    public ValidationSeverity Severity { get; set; } = ValidationSeverity.Error;
    
    public ValidationError(string propertyName, string errorMessage);
    public ValidationError(string propertyName, string errorMessage, string errorCode);
}
```

#### ValidationResult - 验证结果

```csharp
namespace Skywalker.Validation;

public class ValidationResult : IValidationResult
{
    public bool IsValid { get; }
    public IReadOnlyList<ValidationError> Errors { get; }
    
    /// <summary>
    /// 创建成功的验证结果
    /// </summary>
    public static ValidationResult Success();
    
    /// <summary>
    /// 创建失败的验证结果
    /// </summary>
    public static ValidationResult Failure(string propertyName, string errorMessage);
    public static ValidationResult Failure(IEnumerable<ValidationError> errors);
}
```

#### ValidationException - 验证异常

```csharp
namespace Skywalker.Validation;

public class ValidationException : Exception
{
    /// <summary>
    /// 验证错误列表
    /// </summary>
    public IReadOnlyList<ValidationError> Errors { get; }
    
    public ValidationException(string message);
    public ValidationException(IEnumerable<ValidationError> errors);
    public ValidationException(string propertyName, string errorMessage);
    public ValidationException(IValidationResult result);
}
```

#### DataAnnotationsValidator - DataAnnotations 验证器

```csharp
namespace Skywalker.Validation;

/// <summary>
/// 使用 DataAnnotations 特性的验证器
/// </summary>
public class DataAnnotationsValidator : IValidator
{
    public IValidationResult Validate(object instance);
    public Task<IValidationResult> ValidateAsync(object instance, CancellationToken cancellationToken = default);
}

public class DataAnnotationsValidator<T> : DataAnnotationsValidator, IValidator<T> where T : class
{
    public IValidationResult Validate(T instance);
    public Task<IValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default);
}
```

#### CompositeValidator - 组合验证器

```csharp
namespace Skywalker.Validation;

/// <summary>
/// 组合多个验证器的验证器
/// </summary>
public class CompositeValidator : IValidator
{
    public CompositeValidator(IEnumerable<IValidator> validators);
    
    public IValidationResult Validate(object instance);
    public Task<IValidationResult> ValidateAsync(object instance, CancellationToken cancellationToken = default);
}
```

### 服务注册

```csharp
// 使用 DataAnnotations 验证器
services.AddValidation();
```

### 使用示例

```csharp
public class CreateOrderDto
{
    [Required(ErrorMessage = "客户ID不能为空")]
    public Guid CustomerId { get; set; }
    
    [Required(ErrorMessage = "订单项不能为空")]
    [MinLength(1, ErrorMessage = "至少需要一个订单项")]
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderService
{
    private readonly IValidator<CreateOrderDto> _validator;
    
    public OrderService(IValidator<CreateOrderDto> validator)
    {
        _validator = validator;
    }
    
    public async Task<Order> CreateOrderAsync(CreateOrderDto dto)
    {
        var result = await _validator.ValidateAsync(dto);
        if (!result.IsValid)
        {
            throw new ValidationException(result);
        }
        
        // 创建订单...
    }
}
```

