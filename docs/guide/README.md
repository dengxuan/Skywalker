# Skywalker 使用指南

本指南将帮助你快速上手 Skywalker 框架，从环境准备到构建完整的 DDD 应用程序。

---

## 目录

1. [入门指南](#入门指南)
2. [DDD 指南](#ddd-指南)
3. [数据访问指南](#数据访问指南)
4. [基础设施指南](#基础设施指南)
5. [最佳实践](#最佳实践)

---

## 入门指南

### 环境准备

#### 必需环境

| 工具 | 版本要求 | 说明 |
|------|----------|------|
| .NET SDK | 8.0+ | [下载地址](https://dotnet.microsoft.com/download) |
| IDE | VS 2022 / VS Code / Rider | 推荐 VS 2022 或 Rider |

#### 可选环境（根据需要安装）

| 工具 | 用途 | 说明 |
|------|------|------|
| SQL Server | 数据库 | 本地开发可用 LocalDB |
| MySQL | 数据库 | 推荐 8.0+ |
| PostgreSQL | 数据库 | 推荐 14+ |
| Redis | 分布式缓存 | 推荐 6.0+ |
| RabbitMQ | 消息队列 | 用于分布式事件总线 |
| Docker | 容器化 | 快速搭建开发环境 |

#### 使用 Docker 快速搭建开发环境

```bash
# 启动 MySQL
docker run -d --name mysql -p 3306:3306 \
  -e MYSQL_ROOT_PASSWORD=123456 \
  -e MYSQL_DATABASE=skywalker_demo \
  mysql:8.0

# 启动 Redis
docker run -d --name redis -p 6379:6379 redis:7-alpine

# 启动 RabbitMQ（可选）
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 \
  rabbitmq:3-management
```

### 项目创建

#### 方式一：从空项目开始

```bash
# 1. 创建解决方案
mkdir MyApp && cd MyApp
dotnet new sln -n MyApp

# 2. 创建 Web API 项目
dotnet new webapi -n MyApp.Web
dotnet sln add MyApp.Web

# 3. 创建领域层项目
dotnet new classlib -n MyApp.Domain
dotnet sln add MyApp.Domain

# 4. 创建应用层项目
dotnet new classlib -n MyApp.Application
dotnet sln add MyApp.Application

# 5. 创建基础设施层项目
dotnet new classlib -n MyApp.Infrastructure
dotnet sln add MyApp.Infrastructure

# 6. 添加项目引用
cd MyApp.Application
dotnet add reference ../MyApp.Domain
cd ../MyApp.Infrastructure
dotnet add reference ../MyApp.Domain
cd ../MyApp.Web
dotnet add reference ../MyApp.Application
dotnet add reference ../MyApp.Infrastructure
```

#### 方式二：添加到现有项目

```bash
cd MyExistingProject

# 添加核心包
dotnet add package Skywalker.Ddd
dotnet add package Skywalker.Ddd.EntityFrameworkCore
dotnet add package Skywalker.Ddd.AspNetCore

# 根据数据库类型选择
dotnet add package Skywalker.Ddd.EntityFrameworkCore.MySQL
# 或
dotnet add package Skywalker.Ddd.EntityFrameworkCore.SqlServer

# 可选：添加事件总线
dotnet add package Skywalker.EventBus.Local

# 可选：添加缓存
dotnet add package Skywalker.Caching.Redis
```

### 基本配置

#### 1. 配置 appsettings.json

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Port=3306;Database=skywalker_demo;User=root;Password=123456;",
    "Redis": "localhost:6379,abortConnect=false"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

#### 2. 配置 Program.cs

```csharp
using Microsoft.EntityFrameworkCore;
using MyApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// 1. Skywalker 核心服务 + ASP.NET Core 集成
// ============================================
// AddSkywalker()      → 创建 PartManager，发现程序集
// AddUnitOfWork()     → 注册工作单元基础设施
// AddDddDomain()      → 注册领域层（自动注册 IDomainService 实现）
// AddDddApplication() → 注册应用层（自动注册 IApplicationService 实现）
// AddAspNetCore()     → 注册异常处理、响应包装等 Web 特有服务
builder.Services.AddSkywalker()
    .AddUnitOfWork()
    .AddDddDomain()
    .AddDddApplication()
    .AddAspNetCore();

// ============================================
// 2. 数据库配置（自动注册 EF Core 仓储和领域服务）
// ============================================
builder.Services.AddSkywalkerDbContext<AppDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("Default"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Default")));
});

// ============================================
// 3. 事件总线（可选）
// ============================================
builder.Services.AddEventBusLocal();

// ============================================
// 4. ASP.NET Core 服务
// ============================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ============================================
// 5. 启用 Skywalker 中间件（异常处理 + 工作单元）
// ============================================
app.UseSkywalker();

// 开发环境启用 Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

---

## DDD 指南

领域驱动设计（Domain-Driven Design）是 Skywalker 框架的核心设计理念。

### 实体 (Entity)

实体是具有唯一标识的领域对象，其生命周期中标识保持不变。

#### Entity 基类特性

```csharp
// Skywalker.Ddd.Domain.Entities.Entity<TKey> 提供以下特性：
public abstract class Entity<TKey> : IEntity<TKey>, IHasConcurrencyStamp, IHasCreationTime
{
    public virtual TKey Id { get; protected set; }           // 主键
    public virtual string? ConcurrencyStamp { get; set; }    // 乐观并发控制
    public virtual DateTime CreationTime { get; set; }       // 创建时间（自动填充）
}
```

#### 定义实体

```csharp
using Skywalker.Ddd.Domain.Entities;

/// <summary>
/// 订单项实体 - 属于 Order 聚合
/// </summary>
public class OrderItem : Entity<Guid>
{
    /// <summary>
    /// 所属订单ID（外键）
    /// </summary>
    public Guid OrderId { get; private set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// 商品名称（快照）
    /// </summary>
    public string ProductName { get; private set; } = null!;

    /// <summary>
    /// 数量
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// 单价（快照）
    /// </summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>
    /// 小计金额
    /// </summary>
    public decimal Subtotal => Quantity * UnitPrice;

    /// <summary>
    /// EF Core 需要的无参构造函数
    /// </summary>
    protected OrderItem() { }

    /// <summary>
    /// 创建订单项
    /// </summary>
    public OrderItem(Guid id, Guid orderId, Guid productId, string productName, int quantity, decimal unitPrice)
        : base(id)
    {
        if (quantity <= 0)
            throw new ArgumentException("数量必须大于0", nameof(quantity));
        if (unitPrice < 0)
            throw new ArgumentException("单价不能为负", nameof(unitPrice));

        OrderId = orderId;
        ProductId = productId;
        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    /// <summary>
    /// 修改数量
    /// </summary>
    public void ChangeQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("数量必须大于0", nameof(newQuantity));
        Quantity = newQuantity;
    }
}
```

### 聚合根 (Aggregate Root)

聚合根是聚合的入口点，负责维护聚合内部的一致性。外部只能通过聚合根访问聚合内的实体。

#### AggregateRoot 基类特性

```csharp
// Skywalker.Ddd.Domain.Entities.AggregateRoot<TKey> 提供以下特性：
public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey>, IGeneratesDomainEvents
{
    // 继承自 Entity<TKey>：Id, ConcurrencyStamp, CreationTime

    // 领域事件支持
    protected virtual void AddDistributedEvent(object eventData);  // 添加分布式事件
    public virtual IEnumerable<object> GetDistributedEvents();     // 获取所有待发布事件
    public virtual void ClearDistributedEvents();                   // 清除事件
}
```

#### 定义聚合根

```csharp
using Skywalker.Ddd.Domain.Entities;

/// <summary>
/// 订单聚合根
/// </summary>
public class Order : AggregateRoot<Guid>
{
    /// <summary>
    /// 订单编号（业务主键）
    /// </summary>
    public string OrderNo { get; private set; } = null!;

    /// <summary>
    /// 客户ID
    /// </summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// 收货地址
    /// </summary>
    public Address ShippingAddress { get; private set; } = null!;

    /// <summary>
    /// 订单状态
    /// </summary>
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// 订单总金额
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 订单项列表（聚合内的实体集合）
    /// </summary>
    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    /// <summary>
    /// EF Core 需要的无参构造函数
    /// </summary>
    protected Order() { }

    /// <summary>
    /// 创建订单
    /// </summary>
    public Order(Guid id, string orderNo, Guid customerId, Address shippingAddress) : base(id)
    {
        OrderNo = orderNo ?? throw new ArgumentNullException(nameof(orderNo));
        CustomerId = customerId;
        ShippingAddress = shippingAddress ?? throw new ArgumentNullException(nameof(shippingAddress));
        Status = OrderStatus.Pending;
        TotalAmount = 0;
    }

    /// <summary>
    /// 添加订单项
    /// </summary>
    public void AddItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("只有待处理状态的订单可以添加商品");

        // 检查是否已存在相同商品
        var existingItem = _items.FirstOrDefault(x => x.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.ChangeQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var item = new OrderItem(Guid.NewGuid(), Id, productId, productName, quantity, unitPrice);
            _items.Add(item);
        }

        RecalculateTotal();
    }

    /// <summary>
    /// 移除订单项
    /// </summary>
    public void RemoveItem(Guid itemId)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("只有待处理状态的订单可以移除商品");

        var item = _items.FirstOrDefault(x => x.Id == itemId);
        if (item != null)
        {
            _items.Remove(item);
            RecalculateTotal();
        }
    }

    /// <summary>
    /// 确认订单
    /// </summary>
    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("只有待处理状态的订单可以确认");
        if (!_items.Any())
            throw new InvalidOperationException("订单必须包含至少一个商品");

        Status = OrderStatus.Confirmed;

        // 发布领域事件
        AddDistributedEvent(new OrderConfirmedEvent
        {
            OrderId = Id,
            OrderNo = OrderNo,
            CustomerId = CustomerId,
            TotalAmount = TotalAmount,
            ConfirmedTime = DateTime.UtcNow
        });
    }

    /// <summary>
    /// 发货
    /// </summary>
    public void Ship(string trackingNumber)
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("只有已确认的订单可以发货");

        Status = OrderStatus.Shipped;

        AddDistributedEvent(new OrderShippedEvent
        {
            OrderId = Id,
            TrackingNumber = trackingNumber
        });
    }

    /// <summary>
    /// 取消订单
    /// </summary>
    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Completed || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("已完成或已取消的订单不能再次取消");

        Status = OrderStatus.Cancelled;
        Remark = $"取消原因：{reason}";

        AddDistributedEvent(new OrderCancelledEvent
        {
            OrderId = Id,
            Reason = reason
        });
    }

    /// <summary>
    /// 重新计算总金额
    /// </summary>
    private void RecalculateTotal()
    {
        TotalAmount = _items.Sum(x => x.Subtotal);
    }
}

/// <summary>
/// 订单状态枚举
/// </summary>
public enum OrderStatus
{
    Pending = 0,      // 待处理
    Confirmed = 1,    // 已确认
    Shipped = 2,      // 已发货
    Completed = 3,    // 已完成
    Cancelled = 4     // 已取消
}
```

### 值对象 (Value Object)

值对象没有标识，通过属性值来区分，是不可变的。适用于描述领域中的概念，如地址、金额、日期范围等。

```csharp
using Skywalker.Ddd.Domain.ValueObjects;

/// <summary>
/// 地址值对象
/// </summary>
public class Address : ValueObject
{
    public string Province { get; }
    public string City { get; }
    public string District { get; }
    public string Street { get; }
    public string PostalCode { get; }

    protected Address() { } // EF Core

    public Address(string province, string city, string district, string street, string postalCode)
    {
        Province = province ?? throw new ArgumentNullException(nameof(province));
        City = city ?? throw new ArgumentNullException(nameof(city));
        District = district ?? throw new ArgumentNullException(nameof(district));
        Street = street ?? throw new ArgumentNullException(nameof(street));
        PostalCode = postalCode ?? throw new ArgumentNullException(nameof(postalCode));
    }

    /// <summary>
    /// 获取完整地址
    /// </summary>
    public string GetFullAddress() => $"{Province}{City}{District}{Street}";

    /// <summary>
    /// 值对象相等性比较
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Province;
        yield return City;
        yield return District;
        yield return Street;
        yield return PostalCode;
    }
}

/// <summary>
/// 金额值对象
/// </summary>
public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    protected Money() { }

    public Money(decimal amount, string currency = "CNY")
    {
        if (amount < 0)
            throw new ArgumentException("金额不能为负", nameof(amount));
        Amount = amount;
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("不同币种不能直接相加");
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("不同币种不能直接相减");
        return new Money(Amount - other.Amount, Currency);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Currency} {Amount:N2}";
}
```

### 仓储模式 (Repository)

仓储提供对聚合根的持久化访问，隐藏数据访问细节。**只为聚合根创建仓储**。

#### IRepository 接口方法

```csharp
// IRepository<TEntity, TKey> 提供以下方法：

// ========== 查询方法 ==========
Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default);
Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);  // 不存在时抛出异常
Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default);
Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
Task<List<TEntity>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting, CancellationToken cancellationToken = default);

// ========== 计数方法 ==========
Task<int> CountAsync(CancellationToken cancellationToken = default);
Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
Task<long> LongCountAsync(CancellationToken cancellationToken = default);

// ========== 存在性检查 ==========
Task<bool> AnyAsync(TKey id, CancellationToken cancellationToken = default);
Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

// ========== 新增方法 ==========
Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);
Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);

// ========== 更新方法 ==========
Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);
Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);

// ========== 删除方法 ==========
Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);
Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default);
Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);
Task DeleteManyAsync(IEnumerable<TKey> ids, bool autoSave = false, CancellationToken cancellationToken = default);
```

#### 定义仓储接口

```csharp
using Skywalker.Ddd.Domain.Repositories;

/// <summary>
/// 订单仓储接口
/// 继承自 IRepository，获得所有基础 CRUD 方法
/// 可以添加特定于订单的查询方法
/// </summary>
public interface IOrderRepository : IRepository<Order, Guid>
{
    /// <summary>
    /// 根据订单编号获取订单（包含订单项）
    /// </summary>
    Task<Order?> GetByOrderNoAsync(string orderNo, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取客户的订单列表（分页）
    /// </summary>
    Task<List<Order>> GetByCustomerIdAsync(
        Guid customerId,
        int skipCount = 0,
        int maxResultCount = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取指定状态的订单数量
    /// </summary>
    Task<int> GetCountByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
}
```

#### 实现仓储

```csharp
using Microsoft.EntityFrameworkCore;
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
            .Include(o => o.Items)  // 加载订单项
            .FirstOrDefaultAsync(o => o.OrderNo == orderNo, cancellationToken);
    }

    public async Task<List<Order>> GetByCustomerIdAsync(
        Guid customerId,
        int skipCount = 0,
        int maxResultCount = 10,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Orders
            .Include(o => o.Items)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreationTime)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Orders
            .CountAsync(o => o.Status == status, cancellationToken);
    }

    /// <summary>
    /// 重写 GetAsync 以包含导航属性
    /// </summary>
    public override async Task<Order> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        var order = await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        if (order == null)
            throw new EntityNotFoundException(typeof(Order), id);

        return order;
    }
}
```

### 领域服务 (Domain Service)

领域服务用于处理不属于任何单一实体或值对象的领域逻辑，通常涉及多个聚合的协调。

```csharp
/// <summary>
/// 订单领域服务
/// </summary>
public class OrderDomainService : IOrderDomainService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IInventoryService _inventoryService;

    public OrderDomainService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IInventoryService inventoryService)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _inventoryService = inventoryService;
    }

    /// <summary>
    /// 创建订单（涉及多个聚合的业务逻辑）
    /// </summary>
    public async Task<Order> CreateOrderAsync(
        Guid customerId,
        Address shippingAddress,
        List<OrderItemInput> items)
    {
        // 1. 验证商品是否存在且有库存
        foreach (var item in items)
        {
            var product = await _productRepository.GetAsync(item.ProductId);
            var hasStock = await _inventoryService.CheckStockAsync(item.ProductId, item.Quantity);
            if (!hasStock)
                throw new BusinessException($"商品 {product.Name} 库存不足");
        }

        // 2. 创建订单
        var orderNo = GenerateOrderNo();
        var order = new Order(Guid.NewGuid(), orderNo, customerId, shippingAddress);

        // 3. 添加订单项
        foreach (var item in items)
        {
            var product = await _productRepository.GetAsync(item.ProductId);
            order.AddItem(product.Id, product.Name, item.Quantity, product.Price);
        }

        // 4. 预扣库存
        foreach (var item in items)
        {
            await _inventoryService.ReserveStockAsync(item.ProductId, item.Quantity);
        }

        return order;
    }

    private string GenerateOrderNo()
    {
        return $"ORD{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}
```

### 领域事件 (Domain Event)

领域事件用于在聚合之间传递信息，实现松耦合。

#### 定义领域事件

```csharp
/// <summary>
/// 订单确认事件
/// </summary>
public class OrderConfirmedEvent
{
    public Guid OrderId { get; set; }
    public string OrderNo { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime ConfirmedTime { get; set; }
}

/// <summary>
/// 订单发货事件
/// </summary>
public class OrderShippedEvent
{
    public Guid OrderId { get; set; }
    public string TrackingNumber { get; set; } = null!;
}

/// <summary>
/// 订单取消事件
/// </summary>
public class OrderCancelledEvent
{
    public Guid OrderId { get; set; }
    public string Reason { get; set; } = null!;
}
```

#### 在聚合根中发布事件

```csharp
// 在 Order 聚合根的 Confirm 方法中：
public void Confirm()
{
    // ... 业务逻辑 ...

    // 添加领域事件（事件会在 SaveChanges 后自动发布）
    AddDistributedEvent(new OrderConfirmedEvent
    {
        OrderId = Id,
        OrderNo = OrderNo,
        CustomerId = CustomerId,
        TotalAmount = TotalAmount,
        ConfirmedTime = DateTime.UtcNow
    });
}
```

---

## 数据访问指南

### EF Core 配置

#### 配置 DbContext

```csharp
using Microsoft.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore;

public class AppDbContext : SkywalkerDbContext<AppDbContext>
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置 Order 实体
        modelBuilder.Entity<Order>(b =>
        {
            b.ToTable("Orders");
            b.HasKey(x => x.Id);

            // 配置属性
            b.Property(x => x.OrderNo)
                .IsRequired()
                .HasMaxLength(50);
            b.Property(x => x.TotalAmount)
                .HasPrecision(18, 2);
            b.Property(x => x.Remark)
                .HasMaxLength(500);

            // 配置值对象（Owned Entity）
            b.OwnsOne(x => x.ShippingAddress, address =>
            {
                address.Property(a => a.Province).HasMaxLength(50).HasColumnName("ShippingProvince");
                address.Property(a => a.City).HasMaxLength(50).HasColumnName("ShippingCity");
                address.Property(a => a.District).HasMaxLength(50).HasColumnName("ShippingDistrict");
                address.Property(a => a.Street).HasMaxLength(200).HasColumnName("ShippingStreet");
                address.Property(a => a.PostalCode).HasMaxLength(20).HasColumnName("ShippingPostalCode");
            });

            // 配置一对多关系
            b.HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置索引
            b.HasIndex(x => x.OrderNo).IsUnique();
            b.HasIndex(x => x.CustomerId);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.CreationTime);
        });

        // 配置 OrderItem 实体
        modelBuilder.Entity<OrderItem>(b =>
        {
            b.ToTable("OrderItems");
            b.HasKey(x => x.Id);

            b.Property(x => x.ProductName).IsRequired().HasMaxLength(200);
            b.Property(x => x.UnitPrice).HasPrecision(18, 2);

            b.HasIndex(x => x.OrderId);
        });
    }
}
```

### 数据库迁移

```bash
# 添加迁移
dotnet ef migrations add InitialCreate -p MyApp.Infrastructure -s MyApp.Web

# 更新数据库
dotnet ef database update -p MyApp.Infrastructure -s MyApp.Web

# 生成 SQL 脚本（生产环境推荐）
dotnet ef migrations script -p MyApp.Infrastructure -s MyApp.Web -o migrations.sql
```

### 事务管理

#### 使用工作单元

```csharp
using Skywalker.Ddd.Uow.Abstractions;

public class OrderAppService : ApplicationService
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IOrderRepository _orderRepository;
    private readonly IInventoryRepository _inventoryRepository;

    public async Task CreateOrderWithInventoryAsync(CreateOrderInput input)
    {
        // 开启工作单元（手动控制事务）
        using var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
        {
            IsTransactional = true,
            IsolationLevel = IsolationLevel.ReadCommitted
        });

        try
        {
            // 1. 创建订单
            var order = new Order(...);
            await _orderRepository.InsertAsync(order);

            // 2. 扣减库存
            foreach (var item in input.Items)
            {
                var inventory = await _inventoryRepository.GetByProductIdAsync(item.ProductId);
                inventory.Deduct(item.Quantity);
                await _inventoryRepository.UpdateAsync(inventory);
            }

            // 3. 提交事务
            await uow.CompleteAsync();
        }
        catch
        {
            // 事务自动回滚
            throw;
        }
    }
}
```

#### 嵌套工作单元

```csharp
// 外层工作单元
using (var outerUow = _unitOfWorkManager.Begin())
{
    await _orderRepository.InsertAsync(order1);

    // 内层工作单元（共享外层事务）
    using (var innerUow = _unitOfWorkManager.Begin())
    {
        await _orderRepository.InsertAsync(order2);
        await innerUow.CompleteAsync();  // 不会真正提交
    }

    await outerUow.CompleteAsync();  // 这里才真正提交
}
```

---

## 基础设施指南

### 事件总线使用

#### 本地事件总线

本地事件总线（`ILocalEventBus`）用于进程内的事件发布与订阅，基于 `System.Threading.Channels` 实现。

```csharp
// 1. 定义事件
public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public string OrderNo { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
}

// 2. 实现事件处理器
public class OrderCreatedEventHandler : ILocalEventHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;
    private readonly IEmailService _emailService;

    public OrderCreatedEventHandler(
        ILogger<OrderCreatedEventHandler> logger,
        IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task HandleEventAsync(OrderCreatedEvent eventData)
    {
        _logger.LogInformation("处理订单创建事件: {OrderNo}", eventData.OrderNo);

        // 发送订单确认邮件
        await _emailService.SendOrderConfirmationAsync(
            eventData.CustomerId,
            eventData.OrderNo);
    }
}

// 3. 注册本地事件总线（Program.cs）
builder.Services.AddEventBusLocal();

// 4. 发布事件
public class OrderAppService : ApplicationService
{
    private readonly ILocalEventBus _eventBus;

    public async Task CreateAsync(CreateOrderInput input)
    {
        var order = new Order(...);
        await _orderRepository.InsertAsync(order);

        // 发布事件
        await _eventBus.PublishAsync(new OrderCreatedEvent
        {
            OrderId = order.Id,
            OrderNo = order.OrderNo,
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount
        });
    }
}
```

#### 动态订阅/取消订阅

```csharp
// 动态订阅
_eventBus.Subscribe<OrderCreatedEvent, OrderCreatedEventHandler>();

// 动态取消订阅
_eventBus.Unsubscribe<OrderCreatedEvent, OrderCreatedEventHandler>();
```

### 缓存使用

#### 配置缓存

```csharp
// Program.cs — Redis 缓存
builder.Services.AddRedisCaching(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "MyApp:";
});
```

#### 使用缓存

```csharp
using Skywalker.Caching.Abstractions;

public class ProductAppService : ApplicationService
{
    private readonly ICaching _cache;
    private readonly IProductRepository _productRepository;

    public ProductAppService(ICaching cache, IProductRepository productRepository)
    {
        _cache = cache;
        _productRepository = productRepository;
    }

    public async Task<ProductDto?> GetAsync(Guid id)
    {
        var cacheKey = $"product:{id}";

        // 方式1：GetOrSet（推荐）
        var product = await _cache.GetOrSetAsync(cacheKey, async () =>
        {
            return await _productRepository.FindAsync(id);
        });

        return product != null ? ObjectMapper.Map<Product, ProductDto>(product) : null;
    }

    public async Task<List<ProductDto>> GetHotProductsAsync()
    {
        var cacheKey = "products:hot";

        // 方式2：手动 Get/Set
        var products = await _cache.GetAsync<List<Product>>(cacheKey);
        if (products == null)
        {
            products = await _productRepository.GetHotProductsAsync(10);
            await _cache.SetAsync(cacheKey, products, TimeSpan.FromMinutes(5));
        }

        return ObjectMapper.Map<List<Product>, List<ProductDto>>(products);
    }

    public async Task UpdateAsync(Guid id, UpdateProductInput input)
    {
        var product = await _productRepository.GetAsync(id);
        // ... 更新逻辑 ...
        await _productRepository.UpdateAsync(product);

        // 清除缓存
        await _cache.RemoveAsync($"product:{id}");
        await _cache.RemoveAsync("products:hot");
    }
}
```

### 设置管理

#### 定义设置

```csharp
using Skywalker.Settings;

public class AppSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        context.Add(
            new SettingDefinition("App.Name", "Skywalker Demo"),
            new SettingDefinition("App.Theme", "light"),
            new SettingDefinition("App.PageSize", "20"),
            new SettingDefinition("Email.SmtpHost", "smtp.example.com"),
            new SettingDefinition("Email.SmtpPort", "587"),
            new SettingDefinition("Email.EnableSsl", "true", isEncrypted: true)
        );
    }
}
```

#### 使用设置

```csharp
using Skywalker.Settings.Abstractions;

public class EmailService : IEmailService
{
    private readonly ISettingProvider _settingProvider;

    public EmailService(ISettingProvider settingProvider)
    {
        _settingProvider = settingProvider;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        var smtpHost = await _settingProvider.GetAsync("Email.SmtpHost");
        var smtpPort = int.Parse(await _settingProvider.GetAsync("Email.SmtpPort"));
        var enableSsl = bool.Parse(await _settingProvider.GetAsync("Email.EnableSsl"));

        // ... 发送邮件逻辑 ...
    }
}
```

### 权限管理

#### 定义权限

```csharp
using Skywalker.Permissions;

public class AppPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var orderGroup = context.AddGroup("Orders", "订单管理");

        orderGroup.AddPermission("Orders.View", "查看订单");
        orderGroup.AddPermission("Orders.Create", "创建订单");
        orderGroup.AddPermission("Orders.Update", "修改订单");
        orderGroup.AddPermission("Orders.Delete", "删除订单");
        orderGroup.AddPermission("Orders.Cancel", "取消订单");
        orderGroup.AddPermission("Orders.Ship", "订单发货");
    }
}
```

#### 检查权限

```csharp
using Skywalker.Permissions.Abstractions;

public class OrderAppService : ApplicationService
{
    private readonly IPermissionChecker _permissionChecker;

    public async Task<OrderDto> GetAsync(Guid id)
    {
        // 检查权限
        if (!await _permissionChecker.IsGrantedAsync("Orders.View"))
        {
            throw new UnauthorizedAccessException("没有查看订单的权限");
        }

        var order = await _orderRepository.GetAsync(id);
        return ObjectMapper.Map<Order, OrderDto>(order);
    }

    public async Task CancelAsync(Guid id, string reason)
    {
        // 检查多个权限
        var result = await _permissionChecker.IsGrantedAsync(new[]
        {
            "Orders.View",
            "Orders.Cancel"
        });

        if (!result.AllGranted)
        {
            throw new UnauthorizedAccessException("权限不足");
        }

        var order = await _orderRepository.GetAsync(id);
        order.Cancel(reason);
        await _orderRepository.UpdateAsync(order);
    }
}
```

### 本地化

#### 配置本地化资源

```json
// Resources/Localization/zh-CN.json
{
  "Order.Created": "订单已创建",
  "Order.Confirmed": "订单已确认",
  "Order.Shipped": "订单已发货",
  "Order.NotFound": "订单 {0} 不存在",
  "Validation.Required": "{0} 是必填项",
  "Validation.MaxLength": "{0} 长度不能超过 {1} 个字符"
}
```

#### 使用本地化

```csharp
using Skywalker.Localization;

public class OrderAppService : ApplicationService
{
    private readonly IStringLocalizer<OrderResource> _localizer;

    public OrderAppService(IStringLocalizer<OrderResource> localizer)
    {
        _localizer = localizer;
    }

    public async Task<OrderDto> GetAsync(Guid id)
    {
        var order = await _orderRepository.FindAsync(id);
        if (order == null)
        {
            // 使用本地化字符串
            throw new BusinessException(_localizer["Order.NotFound", id]);
        }

        return ObjectMapper.Map<Order, OrderDto>(order);
    }
}
```

---

## 最佳实践

### 项目结构

推荐的项目结构：

```
MyApp/
├── src/
│   ├── MyApp.Domain/                    # 领域层
│   │   ├── Entities/                    # 实体和聚合根
│   │   │   ├── Orders/
│   │   │   │   ├── Order.cs
│   │   │   │   ├── OrderItem.cs
│   │   │   │   └── OrderStatus.cs
│   │   │   └── Products/
│   │   │       └── Product.cs
│   │   ├── ValueObjects/                # 值对象
│   │   │   ├── Address.cs
│   │   │   └── Money.cs
│   │   ├── Repositories/                # 仓储接口
│   │   │   ├── IOrderRepository.cs
│   │   │   └── IProductRepository.cs
│   │   ├── Services/                    # 领域服务
│   │   │   └── OrderDomainService.cs
│   │   └── Events/                      # 领域事件
│   │       ├── OrderCreatedEvent.cs
│   │       └── OrderConfirmedEvent.cs
│   │
│   ├── MyApp.Application/               # 应用层
│   │   ├── Services/                    # 应用服务
│   │   │   ├── IOrderAppService.cs
│   │   │   └── OrderAppService.cs
│   │   ├── Dtos/                        # 数据传输对象
│   │   │   ├── Orders/
│   │   │   │   ├── OrderDto.cs
│   │   │   │   ├── CreateOrderInput.cs
│   │   │   │   └── OrderListDto.cs
│   │   │   └── Products/
│   │   ├── EventHandlers/               # 事件处理器
│   │   │   └── OrderCreatedEventHandler.cs
│   │   └── Validators/                  # 验证器
│   │       └── CreateOrderInputValidator.cs
│   │
│   ├── MyApp.Infrastructure/            # 基础设施层
│   │   ├── Data/                        # 数据访问
│   │   │   ├── AppDbContext.cs
│   │   │   └── Migrations/
│   │   ├── Repositories/                # 仓储实现
│   │   │   ├── OrderRepository.cs
│   │   │   └── ProductRepository.cs
│   │   └── Services/                    # 外部服务实现
│   │       ├── EmailService.cs
│   │       └── SmsService.cs
│   │
│   └── MyApp.Web/                       # 表现层
│       ├── Controllers/
│       │   └── OrdersController.cs
│       ├── Program.cs
│       └── appsettings.json
│
└── tests/
    ├── MyApp.Domain.Tests/
    ├── MyApp.Application.Tests/
    └── MyApp.Web.Tests/
```

### 分层架构规则

| 层 | 可以依赖 | 不能依赖 |
|---|----------|----------|
| Domain | 无 | Application, Infrastructure, Web |
| Application | Domain | Infrastructure, Web |
| Infrastructure | Domain | Application, Web |
| Web | Application, Infrastructure | - |

### 错误处理

```csharp
// 1. 定义业务异常
public class BusinessException : Exception
{
    public string Code { get; }

    public BusinessException(string message, string? code = null) : base(message)
    {
        Code = code ?? "BUSINESS_ERROR";
    }
}

public class EntityNotFoundException : BusinessException
{
    public EntityNotFoundException(Type entityType, object id)
        : base($"{entityType.Name} with id '{id}' was not found.", "ENTITY_NOT_FOUND")
    {
    }
}

// 2. 在聚合根中抛出业务异常
public void Confirm()
{
    if (Status != OrderStatus.Pending)
        throw new BusinessException("只有待处理状态的订单可以确认", "ORDER_INVALID_STATUS");
}

// 3. 全局异常处理（ASP.NET Core）
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        var response = exception switch
        {
            BusinessException bex => new { code = bex.Code, message = bex.Message },
            _ => new { code = "INTERNAL_ERROR", message = "服务器内部错误" }
        };

        context.Response.StatusCode = exception is BusinessException ? 400 : 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response);
    });
});
```

### 日志记录

```csharp
public class OrderAppService : ApplicationService
{
    private readonly ILogger<OrderAppService> _logger;

    public async Task<OrderDto> CreateAsync(CreateOrderInput input)
    {
        _logger.LogInformation("开始创建订单, CustomerId: {CustomerId}", input.CustomerId);

        try
        {
            var order = new Order(...);
            await _orderRepository.InsertAsync(order);

            _logger.LogInformation("订单创建成功, OrderId: {OrderId}, OrderNo: {OrderNo}",
                order.Id, order.OrderNo);

            return ObjectMapper.Map<Order, OrderDto>(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "订单创建失败, CustomerId: {CustomerId}", input.CustomerId);
            throw;
        }
    }
}
```

---

## 更多资源

- [API 文档](../api/README.md) - 详细的 API 参考
- [架构设计](../architecture/README.md) - 框架架构详解
- [示例项目](../../samples/README.md) - 完整示例代码
- [贡献指南](../../CONTRIBUTING.md) - 如何参与贡献

