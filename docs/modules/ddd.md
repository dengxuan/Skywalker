# DDD 核心模块

本文档详细介绍 Skywalker 框架的 DDD（领域驱动设计）核心模块。

---

## 目录

1. [模块概述](#模块概述)
2. [Skywalker.Ddd](#skywalkerddd)
3. [Skywalker.Ddd.Domain](#skywalkerddd-domain)
4. [Skywalker.Ddd.Application](#skywalkerddd-application)
5. [Skywalker.Ddd.Uow](#skywalkerddd-uow)
6. [Skywalker.Ddd.EntityFrameworkCore](#skywalkerddd-entityframeworkcore)
7. [Skywalker.Ddd.AspNetCore](#skywalkerddd-aspnetcore)

---

## 模块概述

| 模块 | NuGet 包 | 说明 |
|------|----------|------|
| Skywalker.Ddd | `Skywalker.Ddd` | DDD 整合包 |
| Skywalker.Ddd.Domain | `Skywalker.Ddd.Domain` | 领域层核心 |
| Skywalker.Ddd.Application | `Skywalker.Ddd.Application` | 应用层核心 |
| Skywalker.Ddd.Uow | `Skywalker.Ddd.Uow` | 工作单元 |
| Skywalker.Ddd.EntityFrameworkCore | `Skywalker.Ddd.EntityFrameworkCore` | EF Core 集成 |
| Skywalker.Ddd.AspNetCore | `Skywalker.Ddd.AspNetCore` | ASP.NET Core 集成 |

### 依赖关系

```
Skywalker.Ddd (整合包)
├── Skywalker.Ddd.Domain
│   └── Skywalker.Ddd.Core
├── Skywalker.Ddd.Application
│   ├── Skywalker.Ddd.Core
│   └── Skywalker.Ddd.Uow
└── Skywalker.Ddd.Uow
    ├── Skywalker.Ddd.Core
    └── Skywalker.Extensions.DynamicProxies

Skywalker.Ddd.EntityFrameworkCore
├── Skywalker.Ddd.Domain
├── Skywalker.Ddd.Uow
└── Microsoft.EntityFrameworkCore
    ├── Skywalker.Ddd.EntityFrameworkCore.MySQL (MySQL)
    └── Skywalker.Ddd.EntityFrameworkCore.SqlServer (SQL Server)
```

---

## Skywalker.Ddd

### 简介

`Skywalker.Ddd` 是 DDD 模块的整合包，引用此包即可使用 Domain、Application、Uow 的所有功能。

### 安装

```bash
dotnet add package Skywalker.Ddd
```

### 注册服务

```csharp
builder.Services.AddSkywalker()
    .AddDdd();
```

---

## Skywalker.Ddd.Domain

### 简介

领域层核心模块，提供实体、聚合根、值对象、仓储接口等 DDD 基础设施。

### 安装

```bash
dotnet add package Skywalker.Ddd.Domain
```

### 核心类型

#### Entity - 实体基类

```csharp
namespace Skywalker.Ddd.Domain.Entities;

[Serializable]
public abstract class Entity : IEntity, IHasConcurrencyStamp, IHasCreationTime
{
    // 乐观并发控制戳
    public virtual string? ConcurrencyStamp { get; set; }
    
    // 创建时间
    public virtual DateTime CreationTime { get; set; }
    
    // 获取主键数组
    public abstract object[] GetKeys();
    
    // 实体相等性比较
    public bool EntityEquals(IEntity other);
}

[Serializable]
public abstract class Entity<TKey> : Entity, IEntity<TKey> where TKey : notnull
{
    public const int MaxIdLength = 40;
    
    // 实体主键
    public virtual TKey Id { get; protected set; }
    
    protected Entity(TKey id) => Id = id;
    protected Entity() : this(default!) { }
}
```

**使用示例：**

```csharp
public class Product : Entity<Guid>
{
    public string Name { get; private set; } = null!;
    public decimal Price { get; private set; }
    
    protected Product() { }
    
    public Product(Guid id, string name, decimal price) : base(id)
    {
        Name = name;
        Price = price;
    }
    
    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new ArgumentException("价格必须大于0");
        Price = newPrice;
    }
}
```

#### AggregateRoot - 聚合根基类

```csharp
namespace Skywalker.Ddd.Domain.Entities;

[Serializable]
public abstract class AggregateRoot<TKey> : Entity<TKey>, 
    IAggregateRoot<TKey>, 
    IGeneratesDomainEvents,
    IHasConcurrencyStamp, 
    IHasCreationTime 
    where TKey : notnull
{
    private readonly ICollection<object> _distributedEvents = new Collection<object>();
    
    // 添加分布式领域事件
    protected virtual void AddDistributedEvent(object eventData)
    {
        _distributedEvents.Add(eventData);
    }
    
    // 获取所有待发布的分布式事件
    public virtual IEnumerable<object> GetDistributedEvents()
    {
        return _distributedEvents;
    }
    
    // 清除所有待发布的分布式事件
    public virtual void ClearDistributedEvents()
    {
        _distributedEvents.Clear();
    }
    
    // 验证聚合根状态
    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return new List<ValidationResult>();
    }
}
```

**使用示例：**

```csharp
public class Order : AggregateRoot<Guid>
{
    public string OrderNo { get; private set; } = null!;
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    protected Order() { }

    public Order(Guid id, string orderNo, Guid customerId) : base(id)
    {
        OrderNo = orderNo;
        CustomerId = customerId;
        Status = OrderStatus.Pending;

        // 发布领域事件
        AddDistributedEvent(new OrderCreatedEvent(id, orderNo, customerId));
    }

    public void AddItem(Guid productId, string productName, decimal price, int quantity)
    {
        var item = new OrderItem(Guid.NewGuid(), Id, productId, productName, price, quantity);
        _items.Add(item);
        RecalculateTotal();
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("只有待处理订单可以确认");

        Status = OrderStatus.Confirmed;
        AddDistributedEvent(new OrderConfirmedEvent(Id));
    }

    private void RecalculateTotal()
    {
        TotalAmount = _items.Sum(x => x.Price * x.Quantity);
    }
}
```

#### 实体接口

| 接口 | 说明 |
|------|------|
| `IEntity` | 实体标记接口，提供 `GetKeys()` 方法 |
| `IEntity<TKey>` | 带主键的实体接口，提供 `Id` 属性 |
| `IAggregateRoot` | 聚合根标记接口 |
| `IAggregateRoot<TKey>` | 带主键的聚合根接口 |
| `IHasConcurrencyStamp` | 提供乐观并发控制戳 |
| `IHasCreationTime` | 提供创建时间属性 |
| `IHasModificationTime` | 提供修改时间属性 |
| `IDeletable` | 软删除接口 |
| `IGeneratesDomainEvents` | 领域事件生成接口 |

### 仓储接口

#### IRepository - 完整仓储接口

```csharp
namespace Skywalker.Ddd.Domain.Repositories;

[UnitOfWork]
public interface IRepository
{
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<long> LongCountAsync(CancellationToken cancellationToken = default);
}

public interface IRepository<TEntity> : IReadOnlyRepository<TEntity>, IBasicRepository<TEntity>
    where TEntity : class, IEntity
{
    // 判断是否存在匹配条件的实体
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    // 获取匹配条件的实体数量
    Task<int> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
    Task<long> LongCountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
}

public interface IRepository<TEntity, TKey> : IRepository<TEntity>, IReadOnlyRepository<TEntity, TKey>, IBasicRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{
    // 判断指定主键的实体是否存在
    Task<bool> AnyAsync(TKey id, CancellationToken cancellationToken = default);
}
```

#### IReadOnlyRepository - 只读仓储接口

```csharp
public interface IReadOnlyRepository<TEntity> : IRepository, IQueryable<TEntity>
    where TEntity : class, IEntity
{
    // 获取所有实体
    Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default);

    // 根据条件获取实体列表
    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

    // 分页查询
    Task<PagedList<TEntity>> GetPagedListAsync(int skip, int limit, string? sorting, string? filter, CancellationToken cancellationToken = default);
    Task<PagedList<TEntity>> GetPagedListAsync(Expression<Func<TEntity, bool>> expression, int skip, int limit, string? sorting, CancellationToken cancellationToken = default);

    // 规约模式查询
    Task<TEntity?> FindAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<PagedList<TEntity>> GetPagedListAsync(ISpecification<TEntity> specification, int skip, int limit, string? sorting = null, CancellationToken cancellationToken = default);
}

public interface IReadOnlyRepository<TEntity, TKey> : IReadOnlyRepository<TEntity>
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{
    // 根据主键获取实体（不存在则抛出异常）
    Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);

    // 根据主键查找实体（不存在返回 null）
    Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default);
}
```

#### IBasicRepository - 基础仓储接口

```csharp
public interface IBasicRepository<TEntity> : IReadOnlyRepository<TEntity>
    where TEntity : class, IEntity
{
    // 插入实体
    Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);
    Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);

    // 更新实体
    Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);
    Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);

    // 删除实体
    Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);
    Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);
}

public interface IBasicRepository<TEntity, TKey> : IBasicRepository<TEntity>, IReadOnlyRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{
    // 根据主键删除实体
    Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default);
    Task DeleteManyAsync(IEnumerable<TKey> ids, bool autoSave = false, CancellationToken cancellationToken = default);
}
```

**仓储使用示例：**

```csharp
public class OrderService
{
    private readonly IRepository<Order, Guid> _orderRepository;

    public OrderService(IRepository<Order, Guid> orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Order> GetOrderAsync(Guid id)
    {
        return await _orderRepository.GetAsync(id);
    }

    public async Task<PagedList<Order>> GetOrdersAsync(int skip, int limit, string? sorting)
    {
        return await _orderRepository.GetPagedListAsync(skip, limit, sorting, null);
    }

    public async Task<Order> CreateOrderAsync(CreateOrderDto dto)
    {
        var order = new Order(Guid.NewGuid(), GenerateOrderNo(), dto.CustomerId);
        foreach (var item in dto.Items)
        {
            order.AddItem(item.ProductId, item.ProductName, item.Price, item.Quantity);
        }
        return await _orderRepository.InsertAsync(order, autoSave: true);
    }

    public async Task ConfirmOrderAsync(Guid orderId)
    {
        var order = await _orderRepository.GetAsync(orderId);
        order.Confirm();
        await _orderRepository.UpdateAsync(order, autoSave: true);
    }
}
```

---

## Skywalker.Ddd.Application

### 简介

应用层核心模块，提供应用服务基类、DTO 定义、CRUD 服务等。

### 安装

```bash
dotnet add package Skywalker.Ddd.Application
```

### 核心类型

#### ApplicationService - 应用服务基类

```csharp
namespace Skywalker.Ddd.Application;

[UnitOfWork]
public abstract class ApplicationService : IApplicationService
{
    // AutoMapper 映射器
    protected IMapper Mapper { get; }

    protected ApplicationService(IMapper mapper) => Mapper = mapper;
}
```

**使用示例：**

```csharp
public class OrderAppService : ApplicationService, IOrderAppService
{
    private readonly IRepository<Order, Guid> _orderRepository;

    public OrderAppService(IMapper mapper, IRepository<Order, Guid> orderRepository)
        : base(mapper)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> GetAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        return Mapper.Map<OrderDto>(order);
    }

    public async Task<PagedResultDto<OrderDto>> GetListAsync(SearchRequestDto input)
    {
        var orders = await _orderRepository.GetPagedListAsync(
            input.Skip,
            input.Limit,
            input.Sorting,
            input.Filter);

        return new PagedResultDto<OrderDto>
        {
            TotalCount = orders.TotalCount,
            Items = Mapper.Map<List<OrderDto>>(orders.Items)
        };
    }
}
```

### DTO 类型

#### 基础 DTO 接口

| 接口 | 说明 |
|------|------|
| `IDto` | DTO 约定接口 |
| `IEntityDto<TKey>` | 实体 DTO，包含 `Id` 属性 |
| `IRequestDto` | 请求 DTO 标记接口 |
| `IResponseDto` | 响应 DTO 标记接口 |

#### 请求 DTO

```csharp
// 限制数量请求
public record LimitedRequestDto(int Limit = 20) : ILimitedRequest, IRequestDto;

// 分页请求
public record PagedRequestDto(int Skip = 0, int Limit = 20) : LimitedRequestDto(Limit), IPagedRequest, IRequestDto;

// 搜索请求（包含过滤、排序、分页）
public record SearchRequestDto(string? Filter, string? Sorting, int Skip = 0, int Limit = 20)
    : PagedRequestDto(Skip, Limit), ISearchRequest, IRequestDto;
```

#### 响应 DTO

```csharp
// 实体 DTO
public record EntityDto<TKey>(TKey Id) : IEntityDto<TKey>, IRequestDto, IResponseDto;

// 分页结果
public record PagedResultDto<T> : IResponseDto, IPagedResponse<T>
{
    public required int TotalCount { get; init; }
    public required IEnumerable<T> Items { get; init; }
}
```

**DTO 使用示例：**

```csharp
// 定义订单 DTO
public record OrderDto(Guid Id, string OrderNo, decimal TotalAmount, OrderStatus Status)
    : EntityDto<Guid>(Id);

// 定义创建订单请求
public record CreateOrderRequest(Guid CustomerId, List<OrderItemRequest> Items) : IRequestDto;

// 定义订单项请求
public record OrderItemRequest(Guid ProductId, string ProductName, decimal Price, int Quantity);

// 在控制器中使用
[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderAppService _orderAppService;

    [HttpGet]
    public async Task<PagedResultDto<OrderDto>> GetList([FromQuery] SearchRequestDto input)
    {
        return await _orderAppService.GetListAsync(input);
    }

    [HttpGet("{id}")]
    public async Task<OrderDto> Get(Guid id)
    {
        return await _orderAppService.GetAsync(id);
    }
}
```

---

## Skywalker.Ddd.Uow

### 简介

工作单元模块，提供事务管理、数据库 API 容器等功能。

### 安装

```bash
dotnet add package Skywalker.Ddd.Uow
```

### 核心类型

#### IUnitOfWork - 工作单元接口

```csharp
namespace Skywalker.Ddd.Uow.Abstractions;

public interface IUnitOfWork : IDatabaseApiContainer, ITransactionApiContainer, IDisposable
{
    // 工作单元唯一标识
    Guid Id { get; }

    // 工作单元选项
    IUnitOfWorkOptions? Options { get; }

    // 外层工作单元（嵌套场景）
    IUnitOfWork? Outer { get; }

    // 是否已预留
    bool IsReserved { get; }

    // 是否已释放
    bool IsDisposed { get; }

    // 是否已完成
    bool IsCompleted { get; }

    // 预留名称
    string? ReservationName { get; }

    // 自定义数据
    Dictionary<string, object> Items { get; }

    // 事件
    event EventHandler<UnitOfWorkFailedEventArgs>? Failed;
    event EventHandler<UnitOfWorkEventArgs>? Disposed;

    // 初始化
    void Initialize(UnitOfWorkOptions options);

    // 预留
    void Reserve(string reservationName);

    // 保存更改
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    // 提交事务
    Task CompleteAsync(CancellationToken cancellationToken = default);

    // 回滚事务
    Task RollbackAsync(CancellationToken cancellationToken = default);

    // 注册完成回调
    void OnCompleted(Func<Task> handler);
}
```

#### IUnitOfWorkManager - 工作单元管理器

```csharp
public interface IUnitOfWorkManager
{
    // 当前工作单元
    IUnitOfWork? Current { get; }

    // 开始新的工作单元
    IUnitOfWork Begin(UnitOfWorkOptions options, bool requiresNew = false);

    // 预留工作单元
    IUnitOfWork Reserve(string reservationName, bool requiresNew = false);

    // 开始预留的工作单元
    void BeginReserved(string reservationName, UnitOfWorkOptions options);

    // 尝试开始预留的工作单元
    bool TryBeginReserved(string reservationName, UnitOfWorkOptions options);
}
```

#### UnitOfWorkOptions - 工作单元选项

```csharp
public class UnitOfWorkOptions : IUnitOfWorkOptions
{
    // 是否启用事务（默认 false）
    public bool IsTransactional { get; set; }

    // 事务隔离级别
    public IsolationLevel? IsolationLevel { get; set; }

    // 超时时间（毫秒）
    public int? Timeout { get; set; }
}
```

#### UnitOfWorkAttribute - 工作单元特性

```csharp
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface)]
public sealed class UnitOfWorkAttribute : Attribute
{
    // 是否启用事务
    public bool? IsTransactional { get; set; }

    // 超时时间（毫秒）
    public int? Timeout { get; set; }

    // 事务隔离级别
    public IsolationLevel? IsolationLevel { get; set; }

    // 是否禁用工作单元
    public bool IsDisabled { get; set; }
}
```

### 注册服务

```csharp
builder.Services.AddSkywalker()
    .AddDynamicProxies()
    .AddUnitOfWork();
```

### 使用示例

#### 自动工作单元（推荐）

```csharp
// 通过 [UnitOfWork] 特性自动管理
[UnitOfWork]
public class OrderAppService : ApplicationService
{
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IRepository<OrderItem, Guid> _orderItemRepository;

    // 方法自动包装在工作单元中
    public async Task CreateOrderAsync(CreateOrderDto dto)
    {
        var order = new Order(Guid.NewGuid(), dto.OrderNo, dto.CustomerId);
        await _orderRepository.InsertAsync(order);

        foreach (var item in dto.Items)
        {
            var orderItem = new OrderItem(Guid.NewGuid(), order.Id, item.ProductId, item.Price, item.Quantity);
            await _orderItemRepository.InsertAsync(orderItem);
        }
        // 方法结束时自动提交
    }

    // 启用事务
    [UnitOfWork(IsTransactional = true)]
    public async Task TransferAsync(Guid fromOrderId, Guid toOrderId, decimal amount)
    {
        // 事务性操作
    }
}
```

#### 手动工作单元

```csharp
public class OrderService
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IRepository<Order, Guid> _orderRepository;

    public async Task CreateOrderAsync(CreateOrderDto dto)
    {
        using var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
        {
            IsTransactional = true,
            IsolationLevel = IsolationLevel.ReadCommitted
        });

        try
        {
            var order = new Order(Guid.NewGuid(), dto.OrderNo, dto.CustomerId);
            await _orderRepository.InsertAsync(order);

            // 提交事务
            await uow.CompleteAsync();
        }
        catch
        {
            // 回滚事务
            await uow.RollbackAsync();
            throw;
        }
    }
}
```

#### 嵌套工作单元

```csharp
public class OrderService
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public async Task ProcessOrdersAsync()
    {
        // 外层工作单元
        using var outerUow = _unitOfWorkManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

        await ProcessOrder1Async();
        await ProcessOrder2Async();

        await outerUow.CompleteAsync();
    }

    private async Task ProcessOrder1Async()
    {
        // 内层工作单元（复用外层）
        using var innerUow = _unitOfWorkManager.Begin(new UnitOfWorkOptions());

        // 操作...

        await innerUow.CompleteAsync(); // 不会真正提交，等待外层提交
    }
}
```

---

## Skywalker.Ddd.EntityFrameworkCore

### 简介

Entity Framework Core 集成模块，提供 EF Core 仓储实现、DbContext 集成等。

### 安装

```bash
# 基础包
dotnet add package Skywalker.Ddd.EntityFrameworkCore

# MySQL 支持
dotnet add package Skywalker.Ddd.EntityFrameworkCore.MySQL

# SQL Server 支持
dotnet add package Skywalker.Ddd.EntityFrameworkCore.SqlServer
```

### 核心类型

#### SkywalkerDbContext - DbContext 基类

```csharp
public abstract class SkywalkerDbContext<TDbContext> : DbContext
    where TDbContext : DbContext
{
    // 工作单元管理器
    protected IUnitOfWorkManager UnitOfWorkManager { get; }

    // 数据过滤器
    protected IDataFilter DataFilter { get; }

    // 配置实体
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 自动配置软删除过滤器
        // 自动配置审计字段
    }
}
```

#### EfCoreRepository - EF Core 仓储实现

```csharp
public class EfCoreRepository<TDbContext, TEntity> : BasicRepository<TEntity>, IRepository<TEntity>
    where TDbContext : DbContext
    where TEntity : class, IEntity
{
    protected TDbContext DbContext { get; }
    protected DbSet<TEntity> DbSet { get; }

    // 实现所有仓储接口方法
}

public class EfCoreRepository<TDbContext, TEntity, TKey> : EfCoreRepository<TDbContext, TEntity>, IRepository<TEntity, TKey>
    where TDbContext : DbContext
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{
    // 实现带主键的仓储接口方法
}
```

### 注册服务

```csharp
// 配置 DbContext
builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// 注册 EF Core 仓储
builder.Services.AddSkywalker()
    .AddDdd()
    .AddEntityFrameworkCore<OrderDbContext>();
```

### 使用示例

#### 定义 DbContext

```csharp
public class OrderDbContext : SkywalkerDbContext<OrderDbContext>
{
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;

    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(b =>
        {
            b.ToTable("Orders");
            b.HasKey(x => x.Id);
            b.Property(x => x.OrderNo).HasMaxLength(50).IsRequired();
            b.Property(x => x.ConcurrencyStamp).HasMaxLength(40).IsConcurrencyToken();

            b.HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey(x => x.OrderId);
        });

        modelBuilder.Entity<OrderItem>(b =>
        {
            b.ToTable("OrderItems");
            b.HasKey(x => x.Id);
        });
    }
}
```

#### 自定义仓储

```csharp
public interface IOrderRepository : IRepository<Order, Guid>
{
    Task<Order?> FindByOrderNoAsync(string orderNo, CancellationToken cancellationToken = default);
    Task<List<Order>> GetPendingOrdersAsync(CancellationToken cancellationToken = default);
}

public class OrderRepository : EfCoreRepository<OrderDbContext, Order, Guid>, IOrderRepository
{
    public OrderRepository(OrderDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Order?> FindByOrderNoAsync(string orderNo, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.OrderNo == orderNo, cancellationToken);
    }

    public async Task<List<Order>> GetPendingOrdersAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(x => x.Status == OrderStatus.Pending)
            .OrderBy(x => x.CreationTime)
            .ToListAsync(cancellationToken);
    }
}
```

---

## Skywalker.Ddd.AspNetCore

### 简介

ASP.NET Core 集成模块，提供控制器基类、异常处理、模型绑定等。

### 安装

```bash
dotnet add package Skywalker.Ddd.AspNetCore
```

### 注册服务

```csharp
builder.Services.AddSkywalker()
    .AddDdd()
    .AddAspNetCore();
```

---

## 最佳实践

### 1. 实体设计

```csharp
// ✅ 好的实践：封装业务逻辑
public class Order : AggregateRoot<Guid>
{
    public OrderStatus Status { get; private set; }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("只有待处理订单可以确认");
        Status = OrderStatus.Confirmed;
        AddDistributedEvent(new OrderConfirmedEvent(Id));
    }
}

// ❌ 不好的实践：贫血模型
public class Order : AggregateRoot<Guid>
{
    public OrderStatus Status { get; set; } // 公开 setter
}
```

### 2. 仓储使用

```csharp
// ✅ 好的实践：使用仓储接口
public class OrderService
{
    private readonly IRepository<Order, Guid> _orderRepository;

    public async Task<Order> GetOrderAsync(Guid id)
    {
        return await _orderRepository.GetAsync(id);
    }
}

// ❌ 不好的实践：直接使用 DbContext
public class OrderService
{
    private readonly OrderDbContext _dbContext;

    public async Task<Order> GetOrderAsync(Guid id)
    {
        return await _dbContext.Orders.FindAsync(id);
    }
}
```

### 3. 工作单元

```csharp
// ✅ 好的实践：使用 [UnitOfWork] 特性
[UnitOfWork(IsTransactional = true)]
public async Task TransferAsync(Guid fromId, Guid toId, decimal amount)
{
    // 自动事务管理
}

// ✅ 好的实践：需要细粒度控制时手动管理
public async Task ComplexOperationAsync()
{
    using var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

    // 操作 1
    await uow.SaveChangesAsync(); // 保存但不提交

    // 操作 2
    await uow.CompleteAsync(); // 提交事务
}
```

---

## 相关文档

- [使用指南 - DDD 开发](../guide/README.md#ddd-开发指南)
- [API 文档 - DDD 核心 API](../api/README.md#ddd-核心-api)
- [架构设计](../architecture/README.md)
```

