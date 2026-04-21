# Skywalker

[![Build Status](https://github.com/L8CHAT/Skywalker/actions/workflows/build.yml/badge.svg)](https://github.com/L8CHAT/Skywalker/actions)
[![NuGet](https://img.shields.io/nuget/v/Skywalker.Ddd.svg)](https://www.nuget.org/packages/Skywalker.Ddd)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.txt)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)

**Skywalker** 是一个轻量级、模块化的 .NET 应用开发框架，基于领域驱动设计（DDD）原则构建，旨在帮助开发者快速构建企业级应用程序。

## ✨ 主要特性

| 特性 | 说明 |
|------|------|
| 🏗️ **领域驱动设计** | 提供完整的 DDD 基础设施：实体（`Entity<TKey>`）、聚合根（`AggregateRoot<TKey>`）、值对象（`ValueObject`）、仓储（`IRepository<TEntity, TKey>`）、领域服务、领域事件 |
| 📦 **模块化架构** | 高度模块化设计，40+ 独立模块，按需引用，灵活组合，降低应用体积 |
| 🔌 **工作单元** | 内置工作单元模式（`IUnitOfWork`），自动管理数据库事务，支持嵌套事务 |
| 📡 **事件总线** | 支持本地事件（`ILocalEventBus`）和分布式事件（RabbitMQ），实现模块间松耦合 |
| 💾 **多数据库支持** | 基于 Entity Framework Core 8.0，支持 MySQL、SQL Server、PostgreSQL、SQLite 等 |
| 🔒 **权限管理** | 灵活的权限系统（`IPermissionChecker`），支持基于角色、用户、客户端的权限检查 |
| ⚙️ **设置管理** | 分层设置系统（`ISettingProvider`），支持全局、租户、用户级别设置，支持加密存储 |
| 🌍 **本地化** | 多语言支持（`IStringLocalizer<T>`），支持 JSON 文件和数据库存储本地化资源 |
| 📝 **模板引擎** | 支持 Scriban（高性能）和 Razor 模板引擎（`ITemplateRenderer`） |
| ✅ **数据验证** | 集成 FluentValidation（`IValidator<T>`）和 DataAnnotations，支持自定义验证规则 |
| 🗺️ **对象映射** | 框架不内置映射器，推荐使用 [Mapperly](https://github.com/riok/mapperly) 源生成器，编译期生成、零运行时开销、AOT 友好 |
| 💨 **缓存** | 支持内存缓存和 Redis 分布式缓存（`ICaching`），统一缓存抽象接口 |
| 🏥 **健康检查** | 内置 ASP.NET Core 健康检查端点，监控应用健康状态 |

## 🛠️ 技术栈

| 技术 | 版本 | 说明 |
|------|------|------|
| **.NET** | 8.0 | 目标框架 |
| **Entity Framework Core** | 8.0 | ORM 框架 |
| **ASP.NET Core** | 8.0 | Web 框架 |
| **FluentValidation** | 11.x | 数据验证 |
| **RabbitMQ.Client** | 6.x | 消息队列（可选） |
| **StackExchange.Redis** | 2.x | Redis 客户端（可选） |
| **Scriban** | 5.x | 模板引擎（可选） |

## 📦 快速开始

### 环境要求

- .NET 8.0 SDK 或更高版本
- Visual Studio 2022 / VS Code / Rider
- SQL Server / MySQL / PostgreSQL（可选）
- Redis（可选）
- RabbitMQ（可选）

### 安装

使用 NuGet 包管理器安装核心包：

```bash
# 安装 DDD 核心包（包含 Domain、Application、Uow）
dotnet add package Skywalker.Ddd

# 安装 Entity Framework Core 集成（根据数据库选择）
dotnet add package Skywalker.Ddd.EntityFrameworkCore
dotnet add package Skywalker.Ddd.EntityFrameworkCore.MySQL
# 或
dotnet add package Skywalker.Ddd.EntityFrameworkCore.SqlServer

# 安装 ASP.NET Core 集成
dotnet add package Skywalker.Ddd.AspNetCore

# 可选：安装事件总线
dotnet add package Skywalker.EventBus.Local

# 可选：安装缓存
dotnet add package Skywalker.Caching.Redis
```

### 基本配置

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. 添加 Skywalker 核心服务 + ASP.NET Core 集成
// ==========================================
// AddSkywalker()  → 注册 UnitOfWork、拦截器等核心基础设施
// AddAspNetCore() → 注册异常处理、响应包装等 Web 特有服务
builder.Services.AddSkywalker()
    .AddAspNetCore();

// ==========================================
// 2. 添加 DbContext（自动注册 EF Core 仓储和领域服务）
// ==========================================
builder.Services.AddSkywalkerDbContext<AppDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("Default"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Default")));
});

// ==========================================
// 3. 添加事件总线（可选）
// ==========================================
builder.Services.AddEventBusLocal();

// ==========================================
// 4. 添加 Redis 缓存（可选）
// ==========================================
builder.Services.AddRedisCaching(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

var app = builder.Build();

// ==========================================
// 5. 启用 Skywalker 中间件（异常处理 + 工作单元）
// ==========================================
app.UseSkywalker();

app.Run();
```

> **控制台 / Worker Service 项目** 不需要 `AddAspNetCore()` 和 `UseSkywalker()`，直接使用 `AddSkywalker()` 即可。

### 配置文件 (appsettings.json)

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=SkywalkerDemo;User=root;Password=123456;",
    "Redis": "localhost:6379"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### 定义聚合根

```csharp
using Skywalker.Ddd.Domain.Entities;

/// <summary>
/// 订单聚合根
/// </summary>
public class Order : AggregateRoot<Guid>
{
    /// <summary>
    /// 订单编号
    /// </summary>
    public string OrderNo { get; private set; } = null!;

    /// <summary>
    /// 客户ID
    /// </summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// 订单总金额
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// 订单项列表
    /// </summary>
    public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();

    /// <summary>
    /// EF Core 需要的无参构造函数
    /// </summary>
    protected Order() { }

    /// <summary>
    /// 创建订单
    /// </summary>
    public Order(Guid id, string orderNo, Guid customerId) : base(id)
    {
        OrderNo = orderNo ?? throw new ArgumentNullException(nameof(orderNo));
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        TotalAmount = 0;
    }

    /// <summary>
    /// 添加订单项
    /// </summary>
    public void AddItem(string productName, int quantity, decimal unitPrice)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("只有待处理订单可以添加商品");

        var item = new OrderItem(Guid.NewGuid(), Id, productName, quantity, unitPrice);
        Items.Add(item);
        RecalculateTotal();
    }

    /// <summary>
    /// 确认订单
    /// </summary>
    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("只有待处理订单可以确认");
        if (!Items.Any())
            throw new InvalidOperationException("订单必须包含至少一个商品");

        Status = OrderStatus.Confirmed;

        // 添加领域事件
        AddDistributedEvent(new OrderConfirmedEvent { OrderId = Id, OrderNo = OrderNo });
    }

    private void RecalculateTotal()
    {
        TotalAmount = Items.Sum(x => x.Quantity * x.UnitPrice);
    }
}

/// <summary>
/// 订单项实体
/// </summary>
public class OrderItem : Entity<Guid>
{
    public Guid OrderId { get; private set; }
    public string ProductName { get; private set; } = null!;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    protected OrderItem() { }

    public OrderItem(Guid id, Guid orderId, string productName, int quantity, decimal unitPrice) : base(id)
    {
        OrderId = orderId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}

public enum OrderStatus
{
    Pending,    // 待处理
    Confirmed,  // 已确认
    Shipped,    // 已发货
    Completed,  // 已完成
    Cancelled   // 已取消
}
```

### 定义仓储接口

```csharp
using Skywalker.Ddd.Domain.Repositories;

/// <summary>
/// 订单仓储接口
/// </summary>
public interface IOrderRepository : IRepository<Order, Guid>
{
    /// <summary>
    /// 根据订单编号获取订单
    /// </summary>
    Task<Order?> GetByOrderNoAsync(string orderNo, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取客户的所有订单
    /// </summary>
    Task<List<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
}
```

### 实现仓储

```csharp
using Skywalker.Ddd.EntityFrameworkCore.Repositories;

/// <summary>
/// 订单仓储实现
/// </summary>
public class OrderRepository : EfCoreRepository<AppDbContext, Order, Guid>, IOrderRepository
{
    public OrderRepository(IDbContextProvider<AppDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<Order?> GetByOrderNoAsync(string orderNo, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderNo == orderNo, cancellationToken);
    }

    public async Task<List<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Orders
            .Include(o => o.Items)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreationTime)
            .ToListAsync(cancellationToken);
    }
}
```

### 定义应用服务

```csharp
using Skywalker.Ddd.Application.Services;

/// <summary>
/// 订单应用服务
/// </summary>
public class OrderAppService : ApplicationService, IOrderAppService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILocalEventBus _eventBus;
    private readonly OrderMapper _mapper = new();

    public OrderAppService(
        IOrderRepository orderRepository,
        ILocalEventBus eventBus)
    {
        _orderRepository = orderRepository;
        _eventBus = eventBus;
    }

    /// <summary>
    /// 创建订单
    /// </summary>
    public async Task<OrderDto> CreateAsync(CreateOrderInput input)
    {
        var order = new Order(
            GuidGenerator.Create(),
            GenerateOrderNo(),
            input.CustomerId);

        foreach (var item in input.Items)
        {
            order.AddItem(item.ProductName, item.Quantity, item.UnitPrice);
        }

        await _orderRepository.InsertAsync(order);

        // 发布订单创建事件
        await _eventBus.PublishAsync(new OrderCreatedEvent
        {
            OrderId = order.Id,
            OrderNo = order.OrderNo,
            CustomerId = order.CustomerId
        });

        return _mapper.ToDto(order);
    }

    /// <summary>
    /// 获取订单详情
    /// </summary>
    public async Task<OrderDto> GetAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        return _mapper.ToDto(order);
    }

    /// <summary>
    /// 确认订单
    /// </summary>
    public async Task ConfirmAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        order.Confirm();
        await _orderRepository.UpdateAsync(order);
    }

    private string GenerateOrderNo()
    {
        return $"ORD{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}
```

## 📚 模块列表

### DDD 核心模块

| 模块 | NuGet 包 | 说明 |
|------|----------|------|
| **DDD 整合包** | `Skywalker.Ddd` | 包含 Domain、Application、Uow 的整合包 |
| **领域层** | `Skywalker.Ddd.Domain` | 实体（`Entity<TKey>`）、聚合根（`AggregateRoot<TKey>`）、仓储接口（`IRepository<TEntity, TKey>`） |
| **应用层** | `Skywalker.Ddd.Application` | 应用服务基类（`ApplicationService`）、DTO、CRUD 服务 |
| **应用层抽象** | `Skywalker.Ddd.Application.Abstractions` | 应用服务接口定义 |
| **工作单元** | `Skywalker.Ddd.Uow` | 工作单元模式（`IUnitOfWork`、`IUnitOfWorkManager`） |

### 数据访问模块

| 模块 | NuGet 包 | 说明 |
|------|----------|------|
| **EF Core 集成** | `Skywalker.Ddd.EntityFrameworkCore` | Entity Framework Core 集成，提供 `EfCoreRepository<TDbContext, TEntity, TKey>` |
| **MySQL 支持** | `Skywalker.Ddd.EntityFrameworkCore.MySQL` | MySQL 数据库支持 |
| **SQL Server 支持** | `Skywalker.Ddd.EntityFrameworkCore.SqlServer` | SQL Server 数据库支持 |

### 事件总线模块

| 模块 | NuGet 包 | 说明 |
|------|----------|------|
| **事件总线抽象** | `Skywalker.EventBus.Abstractions` | 事件总线接口（`IEventBus`、`IEventHandler<TEvent>`） |
| **本地事件总线** | `Skywalker.EventBus.Local` | 进程内事件总线（`ILocalEventBus`），基于 Channel 实现 |
| **RabbitMQ 事件总线** | `Skywalker.EventBus.RabbitMQ` | 分布式事件总线，基于 RabbitMQ |

### 基础设施模块

| 模块 | NuGet 包 | 说明 |
|------|----------|------|
| **缓存抽象** | `Skywalker.Caching.Abstractions` | 缓存接口（`ICaching`、`ICachingProvider`） |
| **Redis 缓存** | `Skywalker.Caching.Redis` | Redis 分布式缓存实现 |
| **设置管理** | `Skywalker.Settings.Abstractions` | 设置系统（`ISettingProvider`），支持全局/租户/用户级别 |
| **权限管理** | `Skywalker.Permissions.Abstractions` | 权限系统（`IPermissionChecker`），支持角色/用户/客户端 |
| **本地化** | `Skywalker.Localization.Abstractions` | 多语言支持（`IStringLocalizer<T>`） |
| **本地化 JSON** | `Skywalker.Localization.Json` | JSON 文件本地化资源 |
| **验证抽象** | `Skywalker.Validation.Abstractions` | 验证接口（`IValidator<T>`） |
| **FluentValidation** | `Skywalker.Validation.FluentValidation` | FluentValidation 集成 |
| **模板抽象** | `Skywalker.Template.Abstractions` | 模板引擎接口（`ITemplateRenderer`） |
| **Scriban 模板** | `Skywalker.Template.Scriban` | Scriban 模板引擎 |
| **Razor 模板** | `Skywalker.Template.Razor` | Razor 模板引擎 |
| **异常处理** | `Skywalker.ExceptionHandler` | 统一异常处理和错误响应 |
| **数据过滤** | `Skywalker.Ddd.Application.DataFiltering` | 软删除、多租户数据过滤 |

### 扩展模块

| 模块 | NuGet 包 | 说明 |
|------|----------|------|
| **邮件服务** | `Skywalker.Extensions.Emailing` | 邮件发送服务 |
| **阿里云短信** | `Skywalker.Sms.Aliyun` | 阿里云短信服务集成 |
| **健康检查** | `Skywalker.HealthChecks.AspNetCore` | ASP.NET Core 健康检查端点 |

### 模块依赖关系

```
Skywalker.Ddd (整合包)
├── Skywalker.Ddd.Domain
│   ├── Skywalker.Ddd.Domain.Abstractions
│   └── Skywalker.Extensions.Universal
├── Skywalker.Ddd.Application
│   ├── Skywalker.Ddd.Application.Abstractions
│   └── Skywalker.Ddd.Domain
└── Skywalker.Ddd.Uow
    └── Skywalker.Ddd.Domain

Skywalker.Ddd.EntityFrameworkCore
├── Skywalker.Ddd.Domain
├── Skywalker.Ddd.Uow
└── Microsoft.EntityFrameworkCore

Skywalker.EventBus.Local
├── Skywalker.EventBus.Abstractions
└── System.Threading.Channels

Skywalker.Caching.Redis
├── Skywalker.Caching.Abstractions
└── StackExchange.Redis
```

## 📖 文档

| 文档 | 说明 |
|------|------|
| [使用指南](docs/guide/README.md) | 入门教程、DDD 指南、数据访问、最佳实践 |
| [API 文档](docs/api/README.md) | 所有公共 API 参考文档 |
| [架构设计](docs/architecture/README.md) | 架构概述、设计决策、扩展指南 |
| [示例项目](samples/README.md) | 完整示例代码和说明 |

## 🤝 贡献

欢迎贡献！请阅读 [贡献指南](CONTRIBUTING.md) 了解如何：

- 报告 Bug
- 提交功能请求
- 提交 Pull Request
- 代码规范

## 📞 联系方式

- **Issues**: [GitHub Issues](https://github.com/L8CHAT/Skywalker/issues)
- **Discussions**: [GitHub Discussions](https://github.com/L8CHAT/Skywalker/discussions)

## 📄 许可证

本项目基于 [MIT 许可证](LICENSE.txt) 开源。

Copyright © 2024 L8CHAT
