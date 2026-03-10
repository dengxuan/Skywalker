# 健康检查模块

本文档详细介绍 Skywalker 框架的健康检查模块。

---

## 目录

1. [模块概述](#模块概述)
2. [Skywalker.HealthChecks.Abstractions](#skywalkerhealthchecksabstractions)
3. [Skywalker.HealthChecks.AspNetCore](#skywalkerhealthchecksaspnetcore)
4. [Skywalker.HealthChecks.Redis](#skywalkerhealthchecksredis)
5. [Skywalker.HealthChecks.RabbitMQ](#skywalkerhealthchecksrabbitmq)
6. [Skywalker.HealthChecks.EntityFrameworkCore](#skywalkerhealthchecksentityframeworkcore)

---

## 模块概述

| 模块 | NuGet 包 | 说明 |
|------|----------|------|
| Skywalker.HealthChecks.Abstractions | `Skywalker.HealthChecks.Abstractions` | 健康检查抽象 |
| Skywalker.HealthChecks.AspNetCore | `Skywalker.HealthChecks.AspNetCore` | ASP.NET Core 端点 |
| Skywalker.HealthChecks.Redis | `Skywalker.HealthChecks.Redis` | Redis 健康检查 |
| Skywalker.HealthChecks.RabbitMQ | `Skywalker.HealthChecks.RabbitMQ` | RabbitMQ 健康检查 |
| Skywalker.HealthChecks.EntityFrameworkCore | `Skywalker.HealthChecks.EntityFrameworkCore` | EF Core 健康检查 |

### 依赖关系

```
Skywalker.HealthChecks.AspNetCore
└── Skywalker.HealthChecks.Abstractions

Skywalker.HealthChecks.Redis
├── Skywalker.HealthChecks.Abstractions
└── Skywalker.Caching.Redis

Skywalker.HealthChecks.RabbitMQ
├── Skywalker.HealthChecks.Abstractions
└── Skywalker.Extensions.RabbitMQ
```

---

## Skywalker.HealthChecks.Abstractions

### 核心类型

#### HealthCheckConsts - 常量定义

```csharp
namespace Skywalker.HealthChecks;

public static class HealthCheckConsts
{
    public const string DefaultEndpoint = "/health";
    public const string DetailedEndpoint = "/health/detail";
    public const string ReadyEndpoint = "/health/ready";
    public const string LiveEndpoint = "/health/live";
    public const int DefaultTimeoutSeconds = 30;
    
    // 标签
    public const string DatabaseTag = "database";
    public const string CacheTag = "cache";
    public const string MessagingTag = "messaging";
    public const string ExternalServiceTag = "external";
    public const string ReadyTag = "ready";
    public const string LiveTag = "live";
}
```

#### HealthCheckResponse - 响应模型

```csharp
namespace Skywalker.HealthChecks;

public class HealthCheckResponse
{
    public string Status { get; set; } = "Healthy";
    public TimeSpan TotalDuration { get; set; }
    public IEnumerable<HealthCheckEntry> Entries { get; set; } = [];
}

public class HealthCheckEntry
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "Healthy";
    public TimeSpan Duration { get; set; }
    public string? Description { get; set; }
    public string? Exception { get; set; }
    public IReadOnlyDictionary<string, object>? Data { get; set; }
    public IEnumerable<string> Tags { get; set; } = [];
}
```

---

## Skywalker.HealthChecks.AspNetCore

### 安装

```bash
dotnet add package Skywalker.HealthChecks.AspNetCore
```

### SkywalkerHealthCheckOptions - 配置选项

```csharp
namespace Skywalker.HealthChecks.AspNetCore;

public class SkywalkerHealthCheckOptions
{
    public string HealthEndpoint { get; set; } = "/health";
    public string DetailedEndpoint { get; set; } = "/health/detail";
    public string ReadyEndpoint { get; set; } = "/health/ready";
    public string LiveEndpoint { get; set; } = "/health/live";
    public bool EnableDetailedEndpoint { get; set; } = true;
    public bool EnableKubernetesEndpoints { get; set; } = true;
    public bool RequireAuthorizationForDetailedEndpoint { get; set; } = false;
}
```

### 服务注册

```csharp
// 添加健康检查服务
services.AddSkywalkerHealthChecks(options =>
{
    options.EnableKubernetesEndpoints = true;
    options.RequireAuthorizationForDetailedEndpoint = true;
});

// 映射端点
app.MapSkywalkerHealthChecks();
```

### 端点说明

| 端点 | 说明 |
|------|------|
| `/health` | 简单健康状态 |
| `/health/detail` | 详细健康信息 |
| `/health/ready` | Kubernetes 就绪探针 |
| `/health/live` | Kubernetes 存活探针 |

---

## Skywalker.HealthChecks.Redis

### 安装

```bash
dotnet add package Skywalker.HealthChecks.Redis
```

### 服务注册

```csharp
services.AddSkywalkerHealthChecks()
    .AddRedisHealthCheck(
        name: "redis",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "cache", "ready" },
        timeout: TimeSpan.FromSeconds(5));
```

### 检查内容

- 连接状态
- Ping 延迟
- 数据库编号

---

## Skywalker.HealthChecks.RabbitMQ

### 安装

```bash
dotnet add package Skywalker.HealthChecks.RabbitMQ
```

### 服务注册

```csharp
services.AddSkywalkerHealthChecks()
    .AddRabbitMqHealthCheck(
        name: "rabbitmq",
        tags: new[] { "messaging", "ready" });
```

### 检查内容

- 连接状态
- 端点信息
- 服务器属性

---

## 使用示例

```csharp
var builder = WebApplication.CreateBuilder(args);

// 添加健康检查
builder.Services.AddSkywalkerHealthChecks()
    .AddRedisHealthCheck()
    .AddRabbitMqHealthCheck()
    .AddDbContextCheck<AppDbContext>();

var app = builder.Build();

// 映射端点
app.MapSkywalkerHealthChecks();

app.Run();
```

