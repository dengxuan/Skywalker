# 扩展模块

本文档详细介绍 Skywalker 框架的扩展模块。

---

## 目录

1. [模块概述](#模块概述)
2. [Skywalker.Extensions.Threading](#skywalkerextensionsthreading)
3. [Skywalker.Extensions.Timezone](#skywalkerextensionstimezone)
4. [Skywalker.Extensions.VirtualFileSystem](#skywalkerextensionsvirtualfilesystem)
5. [Skywalker.Extensions.GuidGenerator](#skywalkerextensionsguidgenerator)
6. [Skywalker.Extensions.RateLimiters](#skywalkerextensionsratelimiters)

---

## 模块概述

| 模块 | NuGet 包 | 说明 |
|------|----------|------|
| Skywalker.Extensions.Threading | `Skywalker.Extensions.Threading` | 线程和异步工具 |
| Skywalker.Extensions.Timezone | `Skywalker.Extensions.Timezone` | 时区处理 |
| Skywalker.Extensions.VirtualFileSystem | `Skywalker.Extensions.VirtualFileSystem` | 虚拟文件系统 |
| Skywalker.Extensions.GuidGenerator | `Skywalker.Extensions.GuidGenerator` | 顺序 GUID 生成 |
| Skywalker.Extensions.RateLimiters | `Skywalker.Extensions.RateLimiters` | 限流器 |

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

