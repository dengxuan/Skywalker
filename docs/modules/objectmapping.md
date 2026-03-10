# 对象映射模块

本文档详细介绍 Skywalker 框架的对象映射模块。

---

## 目录

1. [模块概述](#模块概述)
2. [Skywalker.ObjectMapping.Abstractions](#skywalkerobjectmappingabstractions)
3. [Skywalker.ObjectMapping.AutoMapper](#skywalkerobjectmappingautomapper)

---

## 模块概述

| 模块 | NuGet 包 | 说明 |
|------|----------|------|
| Skywalker.ObjectMapping.Abstractions | `Skywalker.ObjectMapping.Abstractions` | 对象映射抽象接口 |
| Skywalker.ObjectMapping.AutoMapper | `Skywalker.ObjectMapping.AutoMapper` | AutoMapper 集成 |

### 依赖关系

```
Skywalker.ObjectMapping.AutoMapper
├── Skywalker.ObjectMapping.Abstractions
├── AutoMapper (12.0.1)
└── AutoMapper.Extensions.Microsoft.DependencyInjection (12.0.1)
```

---

## Skywalker.ObjectMapping.Abstractions

### 简介

对象映射抽象模块，定义对象映射的核心接口。

### 安装

```bash
dotnet add package Skywalker.ObjectMapping.Abstractions
```

### 核心类型

#### IObjectMapper - 对象映射器接口

```csharp
namespace Skywalker.ObjectMapping;

public interface IObjectMapper
{
    /// <summary>
    /// 将源对象映射到新的目标对象
    /// </summary>
    TDestination Map<TDestination>(object source);
    
    /// <summary>
    /// 将源对象映射到新的目标对象（泛型版本）
    /// </summary>
    TDestination Map<TSource, TDestination>(TSource source);
    
    /// <summary>
    /// 将源对象映射到已存在的目标对象
    /// </summary>
    TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
}

/// <summary>
/// 类型化对象映射器接口
/// </summary>
public interface IObjectMapper<in TSource, TDestination>
{
    /// <summary>
    /// 将源对象映射到新的目标对象
    /// </summary>
    TDestination Map(TSource source);
    
    /// <summary>
    /// 将源对象映射到已存在的目标对象
    /// </summary>
    TDestination Map(TSource source, TDestination destination);
}
```

#### DefaultObjectMapper - 默认对象映射器

```csharp
namespace Skywalker.ObjectMapping;

/// <summary>
/// 使用反射进行简单属性映射的默认对象映射器
/// </summary>
public class DefaultObjectMapper : IObjectMapper
{
    public TDestination Map<TDestination>(object source);
    public TDestination Map<TSource, TDestination>(TSource source);
    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
}
```

### 扩展方法

```csharp
namespace Skywalker.ObjectMapping;

public static class ObjectMapperExtensions
{
    /// <summary>
    /// 映射集合
    /// </summary>
    public static IEnumerable<TDestination> MapCollection<TSource, TDestination>(
        this IObjectMapper mapper,
        IEnumerable<TSource> sources);
}
```

### 服务注册

```csharp
// 使用默认对象映射器
services.AddObjectMapping();
```

### 使用示例

```csharp
public class OrderService
{
    private readonly IObjectMapper _mapper;
    
    public OrderService(IObjectMapper mapper)
    {
        _mapper = mapper;
    }
    
    public OrderDto GetOrder(Order order)
    {
        return _mapper.Map<Order, OrderDto>(order);
    }
    
    public IEnumerable<OrderDto> GetOrders(IEnumerable<Order> orders)
    {
        return _mapper.MapCollection<Order, OrderDto>(orders);
    }
}
```

---

## Skywalker.ObjectMapping.AutoMapper

### 简介

AutoMapper 集成模块，提供强大的对象映射功能。

### 安装

```bash
dotnet add package Skywalker.ObjectMapping.AutoMapper
```

### 核心类型

#### AutoMapperObjectMapper

```csharp
namespace Skywalker.ObjectMapping.AutoMapper;

/// <summary>
/// AutoMapper 实现的对象映射器
/// </summary>
public class AutoMapperObjectMapper : IObjectMapper
{
    private readonly IMapper _mapper;
    
    public AutoMapperObjectMapper(IMapper mapper)
    {
        _mapper = mapper;
    }
    
    public TDestination Map<TDestination>(object source)
        => _mapper.Map<TDestination>(source);
    
    public TDestination Map<TSource, TDestination>(TSource source)
        => _mapper.Map<TSource, TDestination>(source);
    
    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        => _mapper.Map(source, destination);
}
```

### 服务注册

```csharp
// 方式 1：从程序集扫描 Profile
services.AddAutoMapperObjectMapping(typeof(Program).Assembly);

// 方式 2：使用泛型指定程序集
services.AddAutoMapperObjectMapping<Program>();

// 方式 3：使用配置委托
services.AddAutoMapperObjectMapping(cfg =>
{
    cfg.CreateMap<Order, OrderDto>();
    cfg.CreateMap<OrderItem, OrderItemDto>();
});
```

### 使用示例

#### 定义映射配置

```csharp
using AutoMapper;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.CustomerName, 
                       opt => opt.MapFrom(src => src.Customer.Name));
        
        CreateMap<OrderItem, OrderItemDto>();
        
        CreateMap<CreateOrderDto, Order>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}
```

