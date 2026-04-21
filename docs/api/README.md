# Skywalker API 文档

本文档提供 Skywalker 框架所有公共 API 的详细参考。

---

## 目录

1. [DDD 核心 API](#ddd-核心-api)
2. [工作单元 API](#工作单元-api)
3. [事件总线 API](#事件总线-api)
4. [缓存 API](#缓存-api)
5. [设置 API](#设置-api)
6. [权限 API](#权限-api)
7. [本地化 API](#本地化-api)
8. [验证 API](#验证-api)
9. [对象映射 API](#对象映射-api)
10. [模板 API](#模板-api)

---

## DDD 核心 API

### Skywalker.Ddd.Domain.Entities

#### Entity 基类

```csharp
namespace Skywalker.Ddd.Domain.Entities;

/// <summary>
/// 实体基类（无泛型主键）
/// </summary>
public abstract class Entity : IEntity, IHasConcurrencyStamp, IHasCreationTime
{
    /// <summary>
    /// 乐观并发控制戳（自动生成 GUID）
    /// </summary>
    public virtual string? ConcurrencyStamp { get; set; }

    /// <summary>
    /// 创建时间（自动填充）
    /// </summary>
    public virtual DateTime CreationTime { get; set; }

    /// <summary>
    /// 获取实体的所有主键值
    /// </summary>
    public abstract object[] GetKeys();

    /// <summary>
    /// 判断两个实体是否相等（基于主键比较）
    /// </summary>
    public bool EntityEquals(IEntity other);
}

/// <summary>
/// 泛型实体基类
/// </summary>
/// <typeparam name="TKey">主键类型（如 Guid、int、long、string）</typeparam>
public abstract class Entity<TKey> : Entity, IEntity<TKey> where TKey : notnull
{
    /// <summary>
    /// 主键最大长度（默认 40）
    /// </summary>
    public const int MaxIdLength = 40;

    /// <summary>
    /// 实体主键
    /// </summary>
    public virtual TKey Id { get; protected set; }

    /// <summary>
    /// 使用主键创建实体
    /// </summary>
    protected Entity(TKey id);

    /// <summary>
    /// 无参构造函数（EF Core 需要）
    /// </summary>
    protected Entity();
}
```

**使用示例：**

```csharp
// 定义实体
public class Product : Entity<Guid>
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }

    protected Product() { }

    public Product(Guid id, string name, decimal price) : base(id)
    {
        Name = name;
        Price = price;
    }
}
```

#### AggregateRoot 基类

```csharp
namespace Skywalker.Ddd.Domain.Entities;

/// <summary>
/// 聚合根基类
/// </summary>
/// <typeparam name="TKey">主键类型</typeparam>
public abstract class AggregateRoot<TKey> : Entity<TKey>,
    IAggregateRoot<TKey>,
    IGeneratesDomainEvents,
    IHasConcurrencyStamp,
    IHasCreationTime
    where TKey : notnull
{
    /// <summary>
    /// 添加分布式领域事件（在 SaveChanges 后发布）
    /// </summary>
    /// <param name="eventData">事件数据对象</param>
    protected virtual void AddDistributedEvent(object eventData);

    /// <summary>
    /// 获取所有待发布的分布式事件
    /// </summary>
    public virtual IEnumerable<object> GetDistributedEvents();

    /// <summary>
    /// 清除所有待发布的分布式事件
    /// </summary>
    public virtual void ClearDistributedEvents();

    /// <summary>
    /// 验证聚合根状态
    /// </summary>
    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext);
}
```

**使用示例：**

```csharp
public class Order : AggregateRoot<Guid>
{
    public OrderStatus Status { get; private set; }

    public void Confirm()
    {
        Status = OrderStatus.Confirmed;

        // 添加领域事件
        AddDistributedEvent(new OrderConfirmedEvent { OrderId = Id });
    }
}
```

#### 实体接口

| 接口 | 说明 |
|------|------|
| `IEntity` | 实体标记接口 |
| `IEntity<TKey>` | 带主键的实体接口 |
| `IAggregateRoot` | 聚合根标记接口 |
| `IAggregateRoot<TKey>` | 带主键的聚合根接口 |
| `IHasConcurrencyStamp` | 提供并发戳属性 |
| `IHasCreationTime` | 提供创建时间属性 |
| `IHasModificationTime` | 提供修改时间属性 |
| `IDeletable` | 软删除接口 |
| `IGeneratesDomainEvents` | 领域事件生成接口 |

### Skywalker.Ddd.Domain.Repositories

#### IRepository 接口

```csharp
namespace Skywalker.Ddd.Domain.Repositories;

/// <summary>
/// 基础仓储接口
/// </summary>
public interface IRepository
{
    /// <summary>
    /// 获取实体总数
    /// </summary>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取实体总数（long 类型）
    /// </summary>
    Task<long> LongCountAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// 泛型仓储接口
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
public interface IRepository<TEntity> : IReadOnlyRepository<TEntity>, IBasicRepository<TEntity>
    where TEntity : class, IEntity
{
    /// <summary>
    /// 判断是否存在匹配条件的实体
    /// </summary>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用规约判断是否存在匹配的实体
    /// </summary>
    Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取匹配条件的实体数量
    /// </summary>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用规约获取匹配的实体数量
    /// </summary>
    Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
}

/// <summary>
/// 带主键的仓储接口
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TKey">主键类型</typeparam>
public interface IRepository<TEntity, TKey> : IRepository<TEntity>, IReadOnlyRepository<TEntity, TKey>, IBasicRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{
    /// <summary>
    /// 判断指定主键的实体是否存在
    /// </summary>
    Task<bool> AnyAsync(TKey id, CancellationToken cancellationToken = default);
}
```

#### IReadOnlyRepository 接口

```csharp
namespace Skywalker.Ddd.Domain.Repositories;

/// <summary>
/// 只读仓储接口
/// </summary>
public interface IReadOnlyRepository<TEntity> where TEntity : class, IEntity
{
    /// <summary>
    /// 获取所有实体列表
    /// </summary>
    Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据条件获取实体列表
    /// </summary>
    Task<List<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取分页列表
    /// </summary>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大返回数量</param>
    /// <param name="sorting">排序字段（如 "Name ASC" 或 "CreationTime DESC"）</param>
    Task<List<TEntity>> GetPagedListAsync(
        int skipCount,
        int maxResultCount,
        string sorting,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 带主键的只读仓储接口
/// </summary>
public interface IReadOnlyRepository<TEntity, TKey> : IReadOnlyRepository<TEntity>
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{
    /// <summary>
    /// 根据主键查找实体（不存在返回 null）
    /// </summary>
    Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据主键获取实体（不存在抛出 EntityNotFoundException）
    /// </summary>
    Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);
}
```

#### IBasicRepository 接口

```csharp
namespace Skywalker.Ddd.Domain.Repositories;

/// <summary>
/// 基础 CRUD 仓储接口
/// </summary>
public interface IBasicRepository<TEntity> where TEntity : class, IEntity
{
    /// <summary>
    /// 插入实体
    /// </summary>
    /// <param name="entity">实体对象</param>
    /// <param name="autoSave">是否立即保存到数据库</param>
    Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量插入实体
    /// </summary>
    Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新实体
    /// </summary>
    Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量更新实体
    /// </summary>
    Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除实体
    /// </summary>
    Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除实体
    /// </summary>
    Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);
}

/// <summary>
/// 带主键的基础 CRUD 仓储接口
/// </summary>
public interface IBasicRepository<TEntity, TKey> : IBasicRepository<TEntity>
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{
    /// <summary>
    /// 根据主键删除实体
    /// </summary>
    Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据主键批量删除实体
    /// </summary>
    Task DeleteManyAsync(IEnumerable<TKey> ids, bool autoSave = false, CancellationToken cancellationToken = default);
}
```

**使用示例：**

```csharp
// 1. 定义仓储接口
public interface IOrderRepository : IRepository<Order, Guid>
{
    Task<Order?> GetByOrderNoAsync(string orderNo, CancellationToken cancellationToken = default);
}

// 2. 使用仓储
public class OrderAppService
{
    private readonly IOrderRepository _orderRepository;

    public async Task<OrderDto> GetAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);  // 不存在会抛异常
        return MapToDto(order);
    }

    public async Task<OrderDto?> FindByOrderNoAsync(string orderNo)
    {
        var order = await _orderRepository.GetByOrderNoAsync(orderNo);  // 可能返回 null
        return order != null ? MapToDto(order) : null;
    }

    public async Task<List<OrderDto>> GetPagedListAsync(int page, int pageSize)
    {
        var orders = await _orderRepository.GetPagedListAsync(
            skipCount: (page - 1) * pageSize,
            maxResultCount: pageSize,
            sorting: "CreationTime DESC");
        return orders.Select(MapToDto).ToList();
    }

    public async Task CreateAsync(CreateOrderInput input)
    {
        var order = new Order(Guid.NewGuid(), input.OrderNo, input.CustomerId);
        await _orderRepository.InsertAsync(order, autoSave: true);
    }
}
```

---

## 工作单元 API

### Skywalker.Ddd.Uow.Abstractions

#### IUnitOfWork 接口

```csharp
namespace Skywalker.Ddd.Uow.Abstractions;

/// <summary>
/// 工作单元接口
/// </summary>
public interface IUnitOfWork : IDatabaseApiContainer, ITransactionApiContainer, IDisposable
{
    /// <summary>
    /// 工作单元唯一标识
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// 自定义数据字典（用于在工作单元生命周期内传递数据）
    /// </summary>
    Dictionary<string, object> Items { get; }

    /// <summary>
    /// 工作单元配置选项
    /// </summary>
    IUnitOfWorkOptions? Options { get; }

    /// <summary>
    /// 外层工作单元（嵌套场景）
    /// </summary>
    IUnitOfWork? Outer { get; }

    /// <summary>
    /// 是否已预留
    /// </summary>
    bool IsReserved { get; }

    /// <summary>
    /// 是否已释放
    /// </summary>
    bool IsDisposed { get; }

    /// <summary>
    /// 是否已完成
    /// </summary>
    bool IsCompleted { get; }

    /// <summary>
    /// 预留名称
    /// </summary>
    string? ReservationName { get; }

    /// <summary>
    /// 工作单元失败事件
    /// </summary>
    event EventHandler<UnitOfWorkFailedEventArgs>? Failed;

    /// <summary>
    /// 工作单元释放事件
    /// </summary>
    event EventHandler<UnitOfWorkEventArgs>? Disposed;

    /// <summary>
    /// 设置外层工作单元
    /// </summary>
    void SetOuter(IUnitOfWork? outer);

    /// <summary>
    /// 初始化工作单元
    /// </summary>
    void Initialize(UnitOfWorkOptions options);

    /// <summary>
    /// 预留工作单元
    /// </summary>
    void Reserve(string reservationName);

    /// <summary>
    /// 保存更改（不提交事务）
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 完成工作单元（提交事务）
    /// </summary>
    Task CompleteAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 回滚工作单元
    /// </summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 注册完成后回调
    /// </summary>
    void OnCompleted(Func<Task> handler);
}
```

#### IUnitOfWorkManager 接口

```csharp
namespace Skywalker.Ddd.Uow.Abstractions;

/// <summary>
/// 工作单元管理器
/// </summary>
public interface IUnitOfWorkManager
{
    /// <summary>
    /// 当前工作单元
    /// </summary>
    IUnitOfWork? Current { get; }

    /// <summary>
    /// 开始新的工作单元
    /// </summary>
    /// <param name="options">工作单元选项</param>
    /// <param name="requiresNew">是否强制创建新的工作单元</param>
    IUnitOfWork Begin(UnitOfWorkOptions options, bool requiresNew = false);

    /// <summary>
    /// 使用默认选项开始工作单元
    /// </summary>
    IUnitOfWork Begin(bool requiresNew = false);

    /// <summary>
    /// 预留工作单元
    /// </summary>
    IUnitOfWork Reserve(string reservationName, bool requiresNew = false);

    /// <summary>
    /// 开始预留的工作单元
    /// </summary>
    void BeginReserved(string reservationName, UnitOfWorkOptions options);

    /// <summary>
    /// 尝试开始预留的工作单元
    /// </summary>
    bool TryBeginReserved(string reservationName, UnitOfWorkOptions options);
}
```

#### UnitOfWorkOptions 类

```csharp
namespace Skywalker.Ddd.Uow;

/// <summary>
/// 工作单元配置选项
/// </summary>
public class UnitOfWorkOptions
{
    /// <summary>
    /// 是否启用事务（默认 true）
    /// </summary>
    public bool IsTransactional { get; set; } = true;

    /// <summary>
    /// 事务隔离级别
    /// </summary>
    public IsolationLevel? IsolationLevel { get; set; }

    /// <summary>
    /// 超时时间（秒）
    /// </summary>
    public int? Timeout { get; set; }
}
```

**使用示例：**

```csharp
public class OrderAppService
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IOrderRepository _orderRepository;
    private readonly IInventoryRepository _inventoryRepository;

    public async Task CreateOrderWithInventoryAsync(CreateOrderInput input)
    {
        // 开启工作单元
        using var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
        {
            IsTransactional = true,
            IsolationLevel = IsolationLevel.ReadCommitted
        });

        try
        {
            // 创建订单
            var order = new Order(...);
            await _orderRepository.InsertAsync(order);

            // 扣减库存
            var inventory = await _inventoryRepository.GetAsync(input.ProductId);
            inventory.Deduct(input.Quantity);
            await _inventoryRepository.UpdateAsync(inventory);

            // 提交事务
            await uow.CompleteAsync();
        }
        catch
        {
            // 自动回滚
            throw;
        }
    }
}
```

---

## 事件总线 API

### Skywalker.EventBus.Abstractions

#### IEventBus 接口

```csharp
namespace Skywalker.EventBus.Abstractions;

/// <summary>
/// 事件总线接口
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// 发布事件
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <param name="eventData">事件数据</param>
    Task PublishAsync<TEvent>(TEvent eventData) where TEvent : class;

    /// <summary>
    /// 发布事件（非泛型）
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="eventData">事件数据</param>
    Task PublishAsync(Type eventType, object eventData);
}
```

#### IEventHandler 接口

```csharp
namespace Skywalker.EventBus.Abstractions;

/// <summary>
/// 事件处理器接口
/// </summary>
/// <typeparam name="TEvent">事件类型</typeparam>
public interface IEventHandler<in TEvent> where TEvent : class
{
    /// <summary>
    /// 处理事件
    /// </summary>
    /// <param name="eventData">事件数据</param>
    Task HandleEventAsync(TEvent eventData);
}
```

### Skywalker.EventBus.Local

#### ILocalEventBus 接口

```csharp
namespace Skywalker.EventBus.Local;

/// <summary>
/// 本地事件总线接口
/// </summary>
public interface ILocalEventBus : IEventBus
{
    /// <summary>
    /// 订阅事件
    /// </summary>
    void Subscribe<TEvent, THandler>()
        where TEvent : class
        where THandler : IEventHandler<TEvent>;

    /// <summary>
    /// 订阅事件（非泛型）
    /// </summary>
    void Subscribe(Type eventType, Type handlerType);

    /// <summary>
    /// 取消订阅
    /// </summary>
    void Unsubscribe<TEvent, THandler>()
        where TEvent : class
        where THandler : IEventHandler<TEvent>;

    /// <summary>
    /// 取消订阅（非泛型）
    /// </summary>
    void Unsubscribe(Type eventType, Type handlerType);
}
```

#### LocalEventBusOptions 配置

```csharp
namespace Skywalker.EventBus.Local;

/// <summary>
/// 本地事件总线配置
/// </summary>
public class LocalEventBusOptions
{
    /// <summary>
    /// 事件通道容量（默认 1000）
    /// </summary>
    public int ChannelCapacity { get; set; } = 1000;

    /// <summary>
    /// 事件处理器类型列表
    /// </summary>
    public List<Type> Handlers { get; } = new();
}
```

**使用示例：**

```csharp
// 1. 定义事件
public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public string OrderNo { get; set; } = null!;
}

// 2. 定义处理器
public class OrderCreatedEventHandler : ILocalEventHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleEventAsync(OrderCreatedEvent eventData)
    {
        _logger.LogInformation("订单已创建: {OrderNo}", eventData.OrderNo);
        return Task.CompletedTask;
    }
}

// 3. 注册本地事件总线（Program.cs）
builder.Services.AddEventBusLocal();

// 4. 发布事件
public class OrderAppService
{
    private readonly ILocalEventBus _eventBus;

    public async Task CreateAsync(CreateOrderInput input)
    {
        var order = new Order(...);
        await _orderRepository.InsertAsync(order);

        await _eventBus.PublishAsync(new OrderCreatedEvent
        {
            OrderId = order.Id,
            OrderNo = order.OrderNo
        });
    }
}
```

---

## 缓存 API

### Skywalker.Caching.Abstractions

#### ICaching 接口

```csharp
namespace Skywalker.Caching.Abstractions;

/// <summary>
/// 缓存接口
/// </summary>
public interface ICaching : IDisposable
{
    /// <summary>
    /// 缓存名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 默认过期时间（默认 60 分钟）
    /// </summary>
    TimeSpan? DefaultExpireTime { get; set; }

    // ========== 获取方法 ==========

    /// <summary>
    /// 获取缓存（原始字节）
    /// </summary>
    byte[]? Get(string key);

    /// <summary>
    /// 异步获取缓存（原始字节）
    /// </summary>
    ValueTask<byte[]?> GetAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取缓存（泛型）
    /// </summary>
    TValue? Get<TValue>(string key);

    /// <summary>
    /// 异步获取缓存（泛型）
    /// </summary>
    ValueTask<TValue?> GetAsync<TValue>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量获取缓存
    /// </summary>
    IEnumerable<TValue?> GetMany<TValue>(IEnumerable<string> keys);

    /// <summary>
    /// 异步批量获取缓存
    /// </summary>
    ValueTask<IEnumerable<TValue?>> GetManyAsync<TValue>(IEnumerable<string> keys, CancellationToken cancellationToken = default);

    // ========== 设置方法 ==========

    /// <summary>
    /// 设置缓存（原始字节）
    /// </summary>
    void Set(string key, byte[] value, TimeSpan? expireTime = null);

    /// <summary>
    /// 异步设置缓存（原始字节）
    /// </summary>
    Task SetAsync(string key, byte[] value, TimeSpan? expireTime = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置缓存（泛型）
    /// </summary>
    void Set<TValue>(string key, TValue value, TimeSpan? expireTime = null);

    /// <summary>
    /// 异步设置缓存（泛型）
    /// </summary>
    Task SetAsync<TValue>(string key, TValue value, TimeSpan? expireTime = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量设置缓存
    /// </summary>
    void SetMany<TValue>(IEnumerable<KeyValuePair<string, TValue>> items, TimeSpan? expireTime = null);

    /// <summary>
    /// 异步批量设置缓存
    /// </summary>
    Task SetManyAsync<TValue>(IEnumerable<KeyValuePair<string, TValue>> items, TimeSpan? expireTime = null, CancellationToken cancellationToken = default);

    // ========== 获取或设置方法 ==========

    /// <summary>
    /// 获取或设置缓存（缓存不存在时调用 factory 获取并缓存）
    /// </summary>
    TValue GetOrSet<TValue>(string key, Func<TValue> factory);

    /// <summary>
    /// 异步获取或设置缓存
    /// </summary>
    ValueTask<TValue> GetOrSetAsync<TValue>(string key, Func<Task<TValue>> factory, CancellationToken cancellationToken = default);

    // ========== 删除方法 ==========

    /// <summary>
    /// 删除缓存
    /// </summary>
    void Remove(string key);

    /// <summary>
    /// 异步删除缓存
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步批量删除缓存
    /// </summary>
    Task RemoveManyAsync(string[] keys, CancellationToken cancellationToken = default);

    /// <summary>
    /// 清空所有缓存
    /// </summary>
    void Clear();

    /// <summary>
    /// 异步清空所有缓存
    /// </summary>
    Task ClearAsync(CancellationToken cancellationToken = default);
}
```

**使用示例：**

```csharp
public class ProductService
{
    private readonly ICaching _cache;
    private readonly IProductRepository _productRepository;

    public async Task<Product?> GetProductAsync(Guid id)
    {
        var cacheKey = $"product:{id}";

        // 获取或设置（推荐方式）
        return await _cache.GetOrSetAsync(cacheKey, async () =>
        {
            return await _productRepository.FindAsync(id);
        });
    }

    public async Task UpdateProductAsync(Product product)
    {
        await _productRepository.UpdateAsync(product);

        // 清除缓存
        await _cache.RemoveAsync($"product:{product.Id}");
    }

    public async Task<List<Product>> GetHotProductsAsync()
    {
        var cacheKey = "products:hot";

        // 手动获取
        var products = await _cache.GetAsync<List<Product>>(cacheKey);
        if (products == null)
        {
            products = await _productRepository.GetHotProductsAsync(10);

            // 手动设置（5分钟过期）
            await _cache.SetAsync(cacheKey, products, TimeSpan.FromMinutes(5));
        }

        return products;
    }
}
```

---

## 设置 API

### Skywalker.Settings.Abstractions

#### ISettingProvider 接口

```csharp
namespace Skywalker.Settings.Abstractions;

/// <summary>
/// 设置提供者接口
/// </summary>
public interface ISettingProvider
{
    /// <summary>
    /// 获取设置值（不存在时返回默认值）
    /// </summary>
    /// <param name="name">设置名称</param>
    Task<string> GetAsync(string name);

    /// <summary>
    /// 获取设置值（不存在时返回 null）
    /// </summary>
    /// <param name="name">设置名称</param>
    Task<string?> GetOrNullAsync(string name);

    /// <summary>
    /// 批量获取设置值
    /// </summary>
    /// <param name="names">设置名称列表</param>
    Task<List<SettingValue>> GetAllAsync(string[] names);

    /// <summary>
    /// 获取所有设置
    /// </summary>
    Task<List<SettingValue>> GetAllAsync();
}
```

#### SettingValue 类

```csharp
namespace Skywalker.Settings;

/// <summary>
/// 设置值
/// </summary>
public class SettingValue
{
    /// <summary>
    /// 设置名称
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 设置值
    /// </summary>
    public string? Value { get; set; }
}
```

**使用示例：**

```csharp
public class EmailService
{
    private readonly ISettingProvider _settingProvider;

    public async Task SendAsync(string to, string subject, string body)
    {
        // 获取单个设置
        var smtpHost = await _settingProvider.GetAsync("Email.SmtpHost");
        var smtpPort = await _settingProvider.GetOrNullAsync("Email.SmtpPort") ?? "587";

        // 批量获取设置
        var settings = await _settingProvider.GetAllAsync(new[]
        {
            "Email.SmtpHost",
            "Email.SmtpPort",
            "Email.EnableSsl"
        });

        // 使用设置发送邮件...
    }
}
```

---

## 权限 API

### Skywalker.Permissions.Abstractions

#### IPermissionChecker 接口

```csharp
namespace Skywalker.Permissions.Abstractions;

/// <summary>
/// 权限检查器接口
/// </summary>
public interface IPermissionChecker
{
    /// <summary>
    /// 检查当前用户是否拥有指定权限
    /// </summary>
    /// <param name="name">权限名称</param>
    Task<bool> IsGrantedAsync(string name);

    /// <summary>
    /// 检查指定用户是否拥有指定权限
    /// </summary>
    /// <param name="claimsPrincipal">用户声明主体</param>
    /// <param name="name">权限名称</param>
    Task<bool> IsGrantedAsync(ClaimsPrincipal? claimsPrincipal, string name);

    /// <summary>
    /// 批量检查权限
    /// </summary>
    /// <param name="names">权限名称列表</param>
    Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names);

    /// <summary>
    /// 批量检查指定用户的权限
    /// </summary>
    Task<MultiplePermissionGrantResult> IsGrantedAsync(ClaimsPrincipal? claimsPrincipal, string[] names);
}
```

#### MultiplePermissionGrantResult 类

```csharp
namespace Skywalker.Permissions;

/// <summary>
/// 多权限检查结果
/// </summary>
public class MultiplePermissionGrantResult
{
    /// <summary>
    /// 是否全部授权
    /// </summary>
    public bool AllGranted => Result.All(x => x.Value == PermissionGrantResult.Granted);

    /// <summary>
    /// 详细结果字典
    /// </summary>
    public Dictionary<string, PermissionGrantResult> Result { get; }
}

/// <summary>
/// 权限授权结果
/// </summary>
public enum PermissionGrantResult
{
    Undefined,    // 未定义
    Granted,      // 已授权
    Prohibited    // 禁止
}
```

**使用示例：**

```csharp
public class OrderAppService
{
    private readonly IPermissionChecker _permissionChecker;

    public async Task<OrderDto> GetAsync(Guid id)
    {
        // 单个权限检查
        if (!await _permissionChecker.IsGrantedAsync("Orders.View"))
        {
            throw new UnauthorizedAccessException("没有查看订单的权限");
        }

        return await GetOrderAsync(id);
    }

    public async Task DeleteAsync(Guid id)
    {
        // 批量权限检查
        var result = await _permissionChecker.IsGrantedAsync(new[]
        {
            "Orders.View",
            "Orders.Delete"
        });

        if (!result.AllGranted)
        {
            var denied = result.Result
                .Where(x => x.Value != PermissionGrantResult.Granted)
                .Select(x => x.Key);
            throw new UnauthorizedAccessException($"缺少权限: {string.Join(", ", denied)}");
        }

        await DeleteOrderAsync(id);
    }
}
```

---

## 本地化 API

### Skywalker.Localization

#### IStringLocalizer 接口

```csharp
namespace Skywalker.Localization;

/// <summary>
/// 字符串本地化器接口
/// </summary>
public interface IStringLocalizer
{
    /// <summary>
    /// 获取本地化字符串
    /// </summary>
    /// <param name="name">字符串名称</param>
    LocalizedString this[string name] { get; }

    /// <summary>
    /// 获取带参数的本地化字符串
    /// </summary>
    /// <param name="name">字符串名称</param>
    /// <param name="arguments">格式化参数</param>
    LocalizedString this[string name, params object[] arguments] { get; }

    /// <summary>
    /// 获取当前文化的所有本地化字符串
    /// </summary>
    /// <param name="includeParentCultures">是否包含父文化的字符串</param>
    IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures = true);

    /// <summary>
    /// 获取指定文化的所有本地化字符串
    /// </summary>
    IEnumerable<LocalizedString> GetAllStrings(CultureInfo culture, bool includeParentCultures = true);
}

/// <summary>
/// 泛型字符串本地化器接口
/// </summary>
/// <typeparam name="TResource">资源类型</typeparam>
public interface IStringLocalizer<TResource> : IStringLocalizer
{
}
```

#### LocalizedString 类

```csharp
namespace Skywalker.Localization;

/// <summary>
/// 本地化字符串
/// </summary>
public class LocalizedString
{
    /// <summary>
    /// 字符串名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 本地化后的值
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// 是否未找到资源（使用了名称作为值）
    /// </summary>
    public bool ResourceNotFound { get; }
}
```

**使用示例：**

```csharp
// 定义资源类
public class OrderResource { }

// 使用本地化
public class OrderAppService
{
    private readonly IStringLocalizer<OrderResource> _localizer;

    public async Task<OrderDto> GetAsync(Guid id)
    {
        var order = await _orderRepository.FindAsync(id);
        if (order == null)
        {
            // 使用本地化字符串
            throw new BusinessException(_localizer["Order.NotFound", id]);
        }

        return MapToDto(order);
    }
}

// JSON 资源文件 (Resources/Localization/zh-CN.json)
{
  "Order.NotFound": "订单 {0} 不存在",
  "Order.Created": "订单已创建"
}
```

---

## 验证 API

### Skywalker.Validation

#### IValidator 接口

```csharp
namespace Skywalker.Validation;

/// <summary>
/// 验证器接口
/// </summary>
public interface IValidator
{
    /// <summary>
    /// 验证对象
    /// </summary>
    /// <param name="instance">要验证的对象</param>
    IValidationResult Validate(object instance);

    /// <summary>
    /// 异步验证对象
    /// </summary>
    Task<IValidationResult> ValidateAsync(object instance, CancellationToken cancellationToken = default);
}

/// <summary>
/// 泛型验证器接口
/// </summary>
/// <typeparam name="T">要验证的对象类型</typeparam>
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

#### IValidationResult 接口

```csharp
namespace Skywalker.Validation;

/// <summary>
/// 验证结果接口
/// </summary>
public interface IValidationResult
{
    /// <summary>
    /// 是否验证通过
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    /// 验证错误列表
    /// </summary>
    IReadOnlyList<ValidationError> Errors { get; }
}
```

#### ValidationError 类

```csharp
namespace Skywalker.Validation;

/// <summary>
/// 验证错误
/// </summary>
public class ValidationError
{
    /// <summary>
    /// 错误消息
    /// </summary>
    public string ErrorMessage { get; }

    /// <summary>
    /// 属性名称
    /// </summary>
    public string? PropertyName { get; }

    /// <summary>
    /// 错误严重程度
    /// </summary>
    public ValidationSeverity Severity { get; }
}

/// <summary>
/// 验证严重程度
/// </summary>
public enum ValidationSeverity
{
    Error,
    Warning,
    Info
}
```

**使用示例：**

```csharp
// 使用 FluentValidation
public class CreateOrderInputValidator : AbstractValidator<CreateOrderInput>
{
    public CreateOrderInputValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("客户ID不能为空");
        RuleFor(x => x.Items).NotEmpty().WithMessage("订单必须包含至少一个商品");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("数量必须大于0");
            item.RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0).WithMessage("单价不能为负");
        });
    }
}

// 在应用服务中使用
public class OrderAppService
{
    private readonly IValidator<CreateOrderInput> _validator;

    public async Task CreateAsync(CreateOrderInput input)
    {
        var result = await _validator.ValidateAsync(input);
        if (!result.IsValid)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(errors);
        }

        // 继续创建订单...
    }
}
```

---

## 对象映射

框架本身不再内置对象映射器。推荐在业务项目中引用
[Riok.Mapperly](https://github.com/riok/mapperly) 源生成器：声明 `[Mapper] partial class`，
编译期生成映射代码，零运行时反射开销且对原生 AOT 友好。

**使用示例：**

```csharp
using Riok.Mapperly.Abstractions;

[Mapper]
public partial class OrderMapper
{
    public partial OrderDto ToDto(Order entity);
    public partial List<OrderDto> ToDtoList(IEnumerable<Order> entities);
    public partial Order ToEntity(CreateOrderInput input);
}

// 在应用服务中使用
public class OrderAppService : ApplicationService
{
    private readonly OrderMapper _mapper = new();

    public async Task<OrderDto> GetAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        return _mapper.ToDto(order);
    }

    public async Task<List<OrderDto>> GetListAsync()
    {
        var orders = await _orderRepository.GetListAsync();
        return _mapper.ToDtoList(orders);
    }
}
```

---

## 模板 API

### Skywalker.Template.Abstractions

#### ITemplateRenderer 接口

```csharp
namespace Skywalker.Template.Abstractions;

/// <summary>
/// 模板渲染器接口
/// </summary>
public interface ITemplateRenderer
{
    /// <summary>
    /// 渲染模板
    /// </summary>
    /// <param name="templateName">模板名称</param>
    /// <param name="model">模型对象（可选）</param>
    /// <param name="cultureName">文化名称（可选，默认使用当前 UI 文化）</param>
    /// <param name="globalContext">全局上下文字典（可选）</param>
    Task<string> RenderAsync(
        string templateName,
        object? model = null,
        string? cultureName = null,
        Dictionary<string, object>? globalContext = null);
}
```

**使用示例：**

```csharp
// 定义模板 (Templates/OrderConfirmation.tpl)
// Scriban 模板语法
/*
尊敬的 {{ customer_name }}：

您的订单 {{ order_no }} 已确认。

订单详情：
{{ for item in items }}
- {{ item.product_name }} x {{ item.quantity }} = ¥{{ item.subtotal }}
{{ end }}

订单总额：¥{{ total_amount }}

感谢您的购买！
*/

// 使用模板渲染
public class NotificationService
{
    private readonly ITemplateRenderer _templateRenderer;

    public async Task<string> RenderOrderConfirmationAsync(Order order, Customer customer)
    {
        var model = new
        {
            CustomerName = customer.Name,
            OrderNo = order.OrderNo,
            Items = order.Items.Select(i => new
            {
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                Subtotal = i.Subtotal
            }),
            TotalAmount = order.TotalAmount
        };

        return await _templateRenderer.RenderAsync("OrderConfirmation", model);
    }
}
```

---

## 更多资源

- [使用指南](../guide/README.md) - 详细的使用教程
- [架构设计](../architecture/README.md) - 框架架构详解
- [示例项目](../../samples/README.md) - 完整示例代码

