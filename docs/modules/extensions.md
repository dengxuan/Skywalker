# 扩展模块

本文档详细介绍 Skywalker 框架的扩展模块。

---

## 目录

1. [模块概述](#模块概述)
2. [Skywalker.Extensions.DynamicProxies](#skywalkerextensionsdynamicproxies)
3. [Skywalker.Extensions.Threading](#skywalkerextensionsthreading)
4. [Skywalker.Extensions.Timezone](#skywalkerextensionstimezone)
5. [Skywalker.Extensions.VirtualFileSystem](#skywalkerextensionsvirtualfilesystem)
6. [Skywalker.Extensions.GuidGenerator](#skywalkerextensionsguidgenerator)
7. [Skywalker.Extensions.RateLimiters](#skywalkerextensionsratelimiters)

---

## 模块概述

| 模块 | NuGet 包 | 说明 |
|------|----------|------|
| Skywalker.Extensions.DynamicProxies | `Skywalker.Extensions.DynamicProxies` | 动态代理（Castle.DynamicProxy） |
| Skywalker.Extensions.Threading | `Skywalker.Extensions.Threading` | 线程和异步工具 |
| Skywalker.Extensions.Timezone | `Skywalker.Extensions.Timezone` | 时区处理 |
| Skywalker.Extensions.VirtualFileSystem | `Skywalker.Extensions.VirtualFileSystem` | 虚拟文件系统 |
| Skywalker.Extensions.GuidGenerator | `Skywalker.Extensions.GuidGenerator` | 顺序 GUID 生成 |
| Skywalker.Extensions.RateLimiters | `Skywalker.Extensions.RateLimiters` | 限流器 |

---

## Skywalker.Extensions.DynamicProxies

### 简介

动态代理基础设施模块，基于 [Castle.DynamicProxy](https://github.com/castleproject/Core) 实现 AOP 拦截能力。此模块是独立的通用基础设施，不依赖 DDD 或任何业务模块。

### 安装

```bash
dotnet add package Skywalker.Extensions.DynamicProxies
```

### 依赖关系

```
Skywalker.Extensions.DynamicProxies
├── Castle.Core (Castle.DynamicProxy)
└── Microsoft.Extensions.DependencyInjection.Abstractions
```

### 核心类型

#### IInterceptable - 代理标记接口

```csharp
namespace Skywalker.Extensions.DynamicProxies;

/// <summary>
/// 标记接口，表示实现此接口的服务需要启用拦截代理。
/// </summary>
public interface IInterceptable { }
```

实现此接口的服务在调用 `AddInterceptedServices()` 时会被 Castle.DynamicProxy 自动包装为代理实例。**无需任何 DI 标记接口**，只要服务已注册到 `IServiceCollection` 且实现了 `IInterceptable` 即可。

#### IInterceptor - 拦截器接口

```csharp
namespace Skywalker.Extensions.DynamicProxies;

public interface IInterceptor
{
    /// <summary>
    /// 拦截方法调用。
    /// </summary>
    Task InterceptAsync(IMethodInvocation invocation);
}
```

#### IMethodInvocation - 方法调用上下文

```csharp
namespace Skywalker.Extensions.DynamicProxies;

public interface IMethodInvocation
{
    object Target { get; }          // 被代理的目标对象
    MethodInfo Method { get; }      // 被调用的方法
    string MethodName { get; }      // 方法名称
    object?[] Arguments { get; }    // 方法参数
    Type ReturnType { get; }        // 返回类型
    object? ReturnValue { get; set; } // 返回值

    /// <summary>
    /// 继续执行方法调用链（调用下一个拦截器或实际方法）。
    /// </summary>
    Task ProceedAsync();
}
```

#### IInterceptorChainBuilder - 拦截器链构建器

```csharp
namespace Skywalker.Extensions.DynamicProxies;

/// <summary>
/// 类似 ASP.NET Core 的 IApplicationBuilder，用于构建拦截器管道。
/// </summary>
public interface IInterceptorChainBuilder
{
    IServiceProvider ServiceProvider { get; }
    IInterceptorChainBuilder Use(Func<InterceptorDelegate, InterceptorDelegate> middleware);
    InterceptorDelegate Build(InterceptorDelegate target);
}
```

### 服务注册

```csharp
// 方式一：手动注册代理基础服务
services.AddDynamicProxies();

// 方式二：扫描已注册服务并启用代理（推荐，包含 AddDynamicProxies）
services.AddInterceptedServices();
```

> **说明**：各 DDD 层的扩展方法（如 `AddDddDomain()`、`AddDddApplication()`）内部已自动调用 `AddInterceptedServices()`，使用 DDD 模块时无需手动调用。

### 工作原理

`AddInterceptedServices()` 的执行流程：

1. 快照当前 `IServiceCollection` 中所有已注册的 `ServiceDescriptor`
2. 遍历找出 `ImplementationType` 实现了 `IInterceptable` 的服务
3. 确保实现类型本身已注册（代理工厂需要解析原始实例）
4. 用代理工厂替换原有的接口注册，保持原有的生命周期（Singleton/Scoped/Transient）不变

### 使用示例

#### 定义可拦截的服务

```csharp
using Skywalker.Extensions.DynamicProxies;

// 服务接口继承 IInterceptable 启用代理
public interface IOrderService : IInterceptable
{
    Task<Order> CreateAsync(OrderDto dto);
    Task ConfirmAsync(Guid orderId);
}

// 实现类正常实现接口
public class OrderService : IOrderService
{
    public async Task<Order> CreateAsync(OrderDto dto)
    {
        // 业务逻辑
    }

    public async Task ConfirmAsync(Guid orderId)
    {
        // 业务逻辑
    }
}
```

#### 创建自定义拦截器

```csharp
using Skywalker.Extensions.DynamicProxies;

public class LoggingInterceptor : IInterceptor
{
    private readonly ILogger<LoggingInterceptor> _logger;

    public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
    {
        _logger = logger;
    }

    public async Task InterceptAsync(IMethodInvocation invocation)
    {
        _logger.LogInformation("Calling {Method}", invocation.MethodName);

        await invocation.ProceedAsync(); // 调用下一个拦截器或实际方法

        _logger.LogInformation("Completed {Method}", invocation.MethodName);
    }
}
```

#### 使用拦截器链构建器

```csharp
// 通过中间件模式组合拦截器
var chain = builder
    .Use(next => async context =>
    {
        Console.WriteLine("Before");
        await next(context);
        Console.WriteLine("After");
    })
    .UseInterceptor<LoggingInterceptor>()
    .Build();
```

#### 内置拦截器示例：UnitOfWork

```csharp
// UnitOfWorkInterceptor（位于 Skywalker.Ddd.Uow）
// 自动检测方法或类上的 [UnitOfWork] 特性并管理事务

[UnitOfWork]
public interface IOrderService : IInterceptable
{
    Task CreateAsync(OrderDto dto);

    [UnitOfWork(IsTransactional = true)]
    Task TransferAsync(Guid from, Guid to, decimal amount);
}
```

---

## Skywalker.Extensions.Threading

### 核心类型

#### SkywalkerAsyncTimer - 异步定时器

```csharp
namespace Skywalker.Extensions.Threading;

/// <summary>
/// 确保不会重叠执行的定时器
/// </summary>
public class SkywalkerAsyncTimer
{
    /// <summary>
    /// 定时触发的事件
    /// </summary>
    public Func<SkywalkerAsyncTimer, Task> Elapsed { get; set; }

    /// <summary>
    /// 执行周期（毫秒）
    /// </summary>
    public int Period { get; set; }

    /// <summary>
    /// 是否在启动时立即执行一次
    /// </summary>
    public bool RunOnStart { get; set; }

    public Task StartAsync(CancellationToken cancellationToken = default);
    public Task StopAsync(CancellationToken cancellationToken = default);
}
```

#### Lock - 读写锁

```csharp
namespace Skywalker.Extensions.Threading.Locking;

public abstract class Lock
{
    public abstract ILockHolder ForReading();
    public abstract ILockHolder ForWriting();
    public abstract IUpgradeableLockHolder ForReadingUpgradeable();

    public static Lock Create();
}
```

### 服务注册

```csharp
services.AddThreading();
```

---

## Skywalker.Extensions.Timezone

### 核心类型

#### IClock - 时钟接口

```csharp
namespace Skywalker.Extensions.Timezone;

public interface IClock
{
    DateTime Now { get; }
    DateTimeKind Kind { get; }
    DateTime Normalize(DateTime dateTime);
}
```

#### ITimezoneProvider - 时区提供者

```csharp
namespace Skywalker.Extensions.Timezone;

public interface ITimezoneProvider
{
    List<NameValue> GetWindowsTimezones();
    List<NameValue> GetIanaTimezones();
    string WindowsToIana(string windowsTimeZoneId);
    string IanaToWindows(string ianaTimeZoneName);
    TimeZoneInfo GetTimeZoneInfo(string windowsOrIanaTimeZoneId);
}
```

### 服务注册

```csharp
services.AddTimezone();
```

---

## Skywalker.Extensions.VirtualFileSystem

### 核心类型

#### IVirtualFileProvider - 虚拟文件提供者

```csharp
namespace Skywalker.Extensions.VirtualFileSystem;

public interface IVirtualFileProvider : IFileProvider
{
    IFileInfo GetFileInfo(string subpath);
    IDirectoryContents GetDirectoryContents(string subpath);
    IChangeToken Watch(string filter);
}
```

### 配置选项

```csharp
services.Configure<SkywalkerVirtualFileSystemOptions>(options =>
{
    // 添加嵌入式资源
    options.FileSets.AddEmbedded<MyAssemblyMarker>(
        baseNamespace: "MyApp.Resources");

    // 添加物理文件
    options.FileSets.AddPhysical("/path/to/files");
});
```

---

## Skywalker.Extensions.GuidGenerator

### 核心类型

#### IGuidGenerator - GUID 生成器接口

```csharp
namespace Skywalker.Extensions.GuidGenerator;

public interface IGuidGenerator
{
    Guid Create();
}
```

#### SequentialGuidType - 顺序类型

```csharp
public enum SequentialGuidType
{
    SequentialAsString,   // 适用于 MySQL、PostgreSQL
    SequentialAsBinary,   // 适用于 Oracle
    SequentialAtEnd       // 适用于 SQL Server
}
```

### 服务注册

```csharp
services.AddGuidGenerator(options =>
{
    options.DefaultSequentialGuidType = SequentialGuidType.SequentialAtEnd;
});
```

---

## Skywalker.Extensions.RateLimiters

### 核心类型

#### IRateLimiter - 限流器接口

```csharp
namespace Skywalker.Extensions.RateLimiters;

public interface IRateLimiter
{
    string Name { get; }
    RateLimitResult TryAcquire(int permitCount = 1);
    RateLimiterStatistics GetStatistics();
}
```

#### 限流算法

| 类型 | 说明 |
|------|------|
| `FixedWindowRateLimiter` | 固定窗口限流 |
| `TokenBucketRateLimiter` | 令牌桶限流 |
| `LeakyTokenBucket` | 漏桶限流 |

### 使用示例

```csharp
// 固定窗口限流
var limiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
{
    Name = "api-limiter",
    PermitLimit = 100,
    Window = TimeSpan.FromMinutes(1)
});

var result = limiter.TryAcquire();
if (result.IsAcquired)
{
    // 允许请求
}
else
{
    // 拒绝请求，result.RetryAfter 指示重试时间
}
```

