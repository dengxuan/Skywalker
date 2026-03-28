# Skywalker 示例项目

本目录包含 Skywalker 框架的示例代码，帮助你快速了解框架的使用方式。

---

## 目录

1. [示例列表](#示例列表)
2. [快速开始](#快速开始)
3. [完整示例代码](#完整示例代码)
4. [贡献示例](#贡献示例)

---

## 示例列表

### 基础示例

| 示例 | 说明 | 状态 |
|------|------|------|
| BasicCrud | 基本 CRUD 操作示例 | 计划中 |
| DddSample | DDD 分层架构示例 | 计划中 |

### 集成示例

| 示例 | 说明 | 状态 |
|------|------|------|
| RedisCaching | Redis 缓存集成示例 | 计划中 |
| RabbitMQEventBus | RabbitMQ 事件总线示例 | 计划中 |
| AliyunSms | 阿里云短信集成示例 | 计划中 |

### 完整示例

| 示例 | 说明 | 状态 |
|------|------|------|
| FullApplication | 完整应用示例（用户、权限、设置） | 计划中 |

---

## 快速开始

```bash
# 进入示例目录
cd samples/BasicCrud

# 还原依赖
dotnet restore

# 运行示例
dotnet run
```

---

## 完整示例代码

以下是一个完整的电商订单管理示例，展示 Skywalker 框架的核心功能。

### 项目结构

```
OrderManagement/
├── OrderManagement.Domain/           # 领域层
│   ├── Orders/
│   │   ├── Order.cs                  # 订单聚合根
│   │   ├── OrderItem.cs              # 订单项实体
│   │   ├── OrderStatus.cs            # 订单状态枚举
│   │   └── IOrderRepository.cs       # 订单仓储接口
│   └── Products/
│       ├── Product.cs                # 产品聚合根
│       └── IProductRepository.cs     # 产品仓储接口
├── OrderManagement.Application/      # 应用层
│   ├── Orders/
│   │   ├── OrderAppService.cs        # 订单应用服务
│   │   ├── OrderDto.cs               # 订单 DTO
│   │   └── CreateOrderInput.cs       # 创建订单输入
│   └── Products/
│       ├── ProductAppService.cs      # 产品应用服务
│       └── ProductDto.cs             # 产品 DTO
├── OrderManagement.Infrastructure/   # 基础设施层
│   ├── AppDbContext.cs               # 数据库上下文
│   └── Repositories/
│       ├── OrderRepository.cs        # 订单仓储实现
│       └── ProductRepository.cs      # 产品仓储实现
└── OrderManagement.Api/              # 表现层
    ├── Controllers/
    │   ├── OrdersController.cs       # 订单控制器
    │   └── ProductsController.cs     # 产品控制器
    └── Program.cs                    # 启动配置
```

### 1. 领域层 - 聚合根定义

#### Order.cs - 订单聚合根

```csharp
using Skywalker.Ddd.Domain.Entities;

namespace OrderManagement.Domain.Orders;

/// <summary>
/// 订单聚合根
/// </summary>
public class Order : AggregateRoot<Guid>
{
    /// <summary>
    /// 订单号
    /// </summary>
    public string OrderNo { get; private set; } = null!;

    /// <summary>
    /// 客户ID
    /// </summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// 订单状态
    /// </summary>
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// 订单总金额
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// 收货地址
    /// </summary>
    public Address ShippingAddress { get; private set; } = null!;

    /// <summary>
    /// 订单项列表
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
        // 业务规则：只有待处理状态才能添加商品
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("只有待处理订单可以添加商品");
        }

        // 业务规则：数量必须大于0
        if (quantity <= 0)
        {
            throw new ArgumentException("数量必须大于0", nameof(quantity));
        }

        // 业务规则：单价不能为负
        if (unitPrice < 0)
        {
            throw new ArgumentException("单价不能为负", nameof(unitPrice));
        }

        // 业务规则：同一商品合并数量
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
        {
            throw new InvalidOperationException("只有待处理订单可以移除商品");
        }

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
        {
            throw new InvalidOperationException("只有待处理订单可以确认");
        }

        if (!_items.Any())
        {
            throw new InvalidOperationException("订单必须包含至少一个商品");
        }

        Status = OrderStatus.Confirmed;

        // 添加领域事件
        AddDistributedEvent(new OrderConfirmedEvent
        {
            OrderId = Id,
            OrderNo = OrderNo,
            CustomerId = CustomerId,
            TotalAmount = TotalAmount
        });
    }

    /// <summary>
    /// 发货
    /// </summary>
    public void Ship(string trackingNumber)
    {
        if (Status != OrderStatus.Confirmed)
        {
            throw new InvalidOperationException("只有已确认订单可以发货");
        }

        Status = OrderStatus.Shipped;

        AddDistributedEvent(new OrderShippedEvent
        {
            OrderId = Id,
            OrderNo = OrderNo,
            TrackingNumber = trackingNumber
        });
    }

    /// <summary>
    /// 完成订单
    /// </summary>
    public void Complete()
    {
        if (Status != OrderStatus.Shipped)
        {
            throw new InvalidOperationException("只有已发货订单可以完成");
        }

        Status = OrderStatus.Completed;
    }

    /// <summary>
    /// 取消订单
    /// </summary>
    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Completed || Status == OrderStatus.Cancelled)
        {
            throw new InvalidOperationException("已完成或已取消的订单不能再次取消");
        }

        Status = OrderStatus.Cancelled;

        AddDistributedEvent(new OrderCancelledEvent
        {
            OrderId = Id,
            OrderNo = OrderNo,
            Reason = reason
        });
    }

    /// <summary>
    /// 重新计算订单总金额
    /// </summary>
    private void RecalculateTotal()
    {
        TotalAmount = _items.Sum(x => x.Subtotal);
    }
}
```

#### OrderItem.cs - 订单项实体

```csharp
using Skywalker.Ddd.Domain.Entities;

namespace OrderManagement.Domain.Orders;

/// <summary>
/// 订单项实体
/// </summary>
public class OrderItem : Entity<Guid>
{
    /// <summary>
    /// 所属订单ID
    /// </summary>
    public Guid OrderId { get; private set; }

    /// <summary>
    /// 产品ID
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// 产品名称（快照）
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
    /// 小计
    /// </summary>
    public decimal Subtotal => Quantity * UnitPrice;

    protected OrderItem() { }

    public OrderItem(Guid id, Guid orderId, Guid productId, string productName, int quantity, decimal unitPrice) : base(id)
    {
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    /// <summary>
    /// 修改数量
    /// </summary>
    public void ChangeQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
        {
            throw new ArgumentException("数量必须大于0", nameof(newQuantity));
        }

        Quantity = newQuantity;
    }
}
```

#### Address.cs - 地址值对象

```csharp
namespace OrderManagement.Domain.Orders;

/// <summary>
/// 地址值对象
/// </summary>
public class Address : IEquatable<Address>
{
    public string Province { get; }
    public string City { get; }
    public string District { get; }
    public string Street { get; }
    public string PostalCode { get; }
    public string ReceiverName { get; }
    public string ReceiverPhone { get; }

    public Address(
        string province,
        string city,
        string district,
        string street,
        string postalCode,
        string receiverName,
        string receiverPhone)
    {
        Province = province ?? throw new ArgumentNullException(nameof(province));
        City = city ?? throw new ArgumentNullException(nameof(city));
        District = district ?? throw new ArgumentNullException(nameof(district));
        Street = street ?? throw new ArgumentNullException(nameof(street));
        PostalCode = postalCode ?? throw new ArgumentNullException(nameof(postalCode));
        ReceiverName = receiverName ?? throw new ArgumentNullException(nameof(receiverName));
        ReceiverPhone = receiverPhone ?? throw new ArgumentNullException(nameof(receiverPhone));
    }

    public string FullAddress => $"{Province}{City}{District}{Street}";

    public bool Equals(Address? other)
    {
        if (other is null) return false;
        return Province == other.Province
            && City == other.City
            && District == other.District
            && Street == other.Street
            && PostalCode == other.PostalCode;
    }

    public override bool Equals(object? obj) => Equals(obj as Address);

    public override int GetHashCode() => HashCode.Combine(Province, City, District, Street, PostalCode);
}
```

#### IOrderRepository.cs - 仓储接口

```csharp
using Skywalker.Ddd.Domain.Repositories;

namespace OrderManagement.Domain.Orders;

/// <summary>
/// 订单仓储接口
/// </summary>
public interface IOrderRepository : IRepository<Order, Guid>
{
    /// <summary>
    /// 根据订单号获取订单
    /// </summary>
    Task<Order?> GetByOrderNoAsync(string orderNo, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取客户的订单列表
    /// </summary>
    Task<List<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取指定状态的订单列表
    /// </summary>
    Task<List<Order>> GetByStatusAsync(OrderStatus status, int maxCount = 100, CancellationToken cancellationToken = default);
}
```

### 2. 应用层 - 应用服务

#### OrderAppService.cs

```csharp
using Skywalker.Ddd.Application.Services;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.EventBus.Local;
using Skywalker.ObjectMapping;

namespace OrderManagement.Application.Orders;

/// <summary>
/// 订单应用服务
/// </summary>
public class OrderAppService : ApplicationService, IOrderAppService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly ILocalEventBus _eventBus;

    public OrderAppService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IUnitOfWorkManager unitOfWorkManager,
        ILocalEventBus eventBus)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWorkManager = unitOfWorkManager;
        _eventBus = eventBus;
    }

    /// <summary>
    /// 获取订单详情
    /// </summary>
    public async Task<OrderDto> GetAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        return ObjectMapper.Map<Order, OrderDto>(order);
    }

    /// <summary>
    /// 根据订单号获取订单
    /// </summary>
    public async Task<OrderDto?> GetByOrderNoAsync(string orderNo)
    {
        var order = await _orderRepository.GetByOrderNoAsync(orderNo);
        return order != null ? ObjectMapper.Map<Order, OrderDto>(order) : null;
    }

    /// <summary>
    /// 获取客户的订单列表
    /// </summary>
    public async Task<List<OrderDto>> GetByCustomerIdAsync(Guid customerId)
    {
        var orders = await _orderRepository.GetByCustomerIdAsync(customerId);
        return ObjectMapper.Map<List<Order>, List<OrderDto>>(orders);
    }

    /// <summary>
    /// 创建订单
    /// </summary>
    public async Task<OrderDto> CreateAsync(CreateOrderInput input)
    {
        // 生成订单号
        var orderNo = $"ORD{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";

        // 创建地址值对象
        var address = new Address(
            input.ShippingAddress.Province,
            input.ShippingAddress.City,
            input.ShippingAddress.District,
            input.ShippingAddress.Street,
            input.ShippingAddress.PostalCode,
            input.ShippingAddress.ReceiverName,
            input.ShippingAddress.ReceiverPhone);

        // 创建订单
        var order = new Order(Guid.NewGuid(), orderNo, input.CustomerId, address);

        // 添加订单项
        foreach (var item in input.Items)
        {
            var product = await _productRepository.GetAsync(item.ProductId);
            order.AddItem(product.Id, product.Name, item.Quantity, product.Price);
        }

        // 保存订单
        await _orderRepository.InsertAsync(order, autoSave: true);

        // 发布订单创建事件
        await _eventBus.PublishAsync(new OrderCreatedEvent
        {
            OrderId = order.Id,
            OrderNo = order.OrderNo,
            CustomerId = order.CustomerId
        });

        return ObjectMapper.Map<Order, OrderDto>(order);
    }

    /// <summary>
    /// 确认订单
    /// </summary>
    public async Task ConfirmAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        order.Confirm();
        await _orderRepository.UpdateAsync(order, autoSave: true);
    }

    /// <summary>
    /// 发货
    /// </summary>
    public async Task ShipAsync(Guid id, string trackingNumber)
    {
        var order = await _orderRepository.GetAsync(id);
        order.Ship(trackingNumber);
        await _orderRepository.UpdateAsync(order, autoSave: true);
    }

    /// <summary>
    /// 取消订单
    /// </summary>
    public async Task CancelAsync(Guid id, string reason)
    {
        var order = await _orderRepository.GetAsync(id);
        order.Cancel(reason);
        await _orderRepository.UpdateAsync(order, autoSave: true);
    }
}
```

### 3. 基础设施层 - 仓储实现

#### OrderRepository.cs

```csharp
using Microsoft.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore.Repositories;

namespace OrderManagement.Infrastructure.Repositories;

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

    public async Task<List<Order>> GetByStatusAsync(OrderStatus status, int maxCount = 100, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Orders
            .Include(o => o.Items)
            .Where(o => o.Status == status)
            .OrderBy(o => o.CreationTime)
            .Take(maxCount)
            .ToListAsync(cancellationToken);
    }

    public override async Task<Order> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        var order = await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        if (order == null)
        {
            throw new EntityNotFoundException(typeof(Order), id);
        }

        return order;
    }
}
```

### 4. 表现层 - API 控制器

#### OrdersController.cs

```csharp
using Microsoft.AspNetCore.Mvc;

namespace OrderManagement.Api.Controllers;

/// <summary>
/// 订单 API 控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderAppService _orderAppService;

    public OrdersController(IOrderAppService orderAppService)
    {
        _orderAppService = orderAppService;
    }

    /// <summary>
    /// 获取订单详情
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> Get(Guid id)
    {
        var order = await _orderAppService.GetAsync(id);
        return Ok(order);
    }

    /// <summary>
    /// 根据订单号获取订单
    /// </summary>
    [HttpGet("by-order-no/{orderNo}")]
    public async Task<ActionResult<OrderDto>> GetByOrderNo(string orderNo)
    {
        var order = await _orderAppService.GetByOrderNoAsync(orderNo);
        if (order == null)
        {
            return NotFound();
        }
        return Ok(order);
    }

    /// <summary>
    /// 获取客户的订单列表
    /// </summary>
    [HttpGet("by-customer/{customerId:guid}")]
    public async Task<ActionResult<List<OrderDto>>> GetByCustomerId(Guid customerId)
    {
        var orders = await _orderAppService.GetByCustomerIdAsync(customerId);
        return Ok(orders);
    }

    /// <summary>
    /// 创建订单
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderInput input)
    {
        var order = await _orderAppService.CreateAsync(input);
        return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
    }

    /// <summary>
    /// 确认订单
    /// </summary>
    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        await _orderAppService.ConfirmAsync(id);
        return NoContent();
    }

    /// <summary>
    /// 发货
    /// </summary>
    [HttpPost("{id:guid}/ship")]
    public async Task<IActionResult> Ship(Guid id, [FromBody] ShipOrderInput input)
    {
        await _orderAppService.ShipAsync(id, input.TrackingNumber);
        return NoContent();
    }

    /// <summary>
    /// 取消订单
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelOrderInput input)
    {
        await _orderAppService.CancelAsync(id, input.Reason);
        return NoContent();
    }
}
```

### 5. 启动配置

#### Program.cs

```csharp
using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Orders;
using OrderManagement.Domain.Orders;
using OrderManagement.Infrastructure;
using OrderManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Skywalker 核心 + ASP.NET Core 集成
builder.Services.AddSkywalker().AddAspNetCore();

// 2. 数据库配置（自动注册 EF Core 仓储）
builder.Services.AddSkywalkerDbContext<AppDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("Default"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Default")));
});

// 3. 本地事件总线
builder.Services.AddEventBusLocal();

// 4. ASP.NET Core 服务
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5. 启用 Skywalker 中间件（异常处理 + 工作单元）
app.UseSkywalker();

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

### 6. 事件处理器

#### OrderCreatedEventHandler.cs

```csharp
using Skywalker.EventBus.Local;

namespace OrderManagement.Application.Orders.EventHandlers;

/// <summary>
/// 订单创建事件处理器
/// </summary>
public class OrderCreatedEventHandler : ILocalEventHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleEventAsync(OrderCreatedEvent eventData)
    {
        _logger.LogInformation(
            "订单已创建 - OrderId: {OrderId}, OrderNo: {OrderNo}, CustomerId: {CustomerId}",
            eventData.OrderId,
            eventData.OrderNo,
            eventData.CustomerId);

        // 可以在这里发送通知、更新统计等

        return Task.CompletedTask;
    }
}

/// <summary>
/// 订单确认事件处理器
/// </summary>
public class OrderConfirmedEventHandler : ILocalEventHandler<OrderConfirmedEvent>
{
    private readonly ILogger<OrderConfirmedEventHandler> _logger;
    private readonly IInventoryService _inventoryService;

    public OrderConfirmedEventHandler(
        ILogger<OrderConfirmedEventHandler> logger,
        IInventoryService inventoryService)
    {
        _logger = logger;
        _inventoryService = inventoryService;
    }

    public async Task HandleEventAsync(OrderConfirmedEvent eventData)
    {
        _logger.LogInformation(
            "订单已确认 - OrderId: {OrderId}, OrderNo: {OrderNo}, TotalAmount: {TotalAmount}",
            eventData.OrderId,
            eventData.OrderNo,
            eventData.TotalAmount);

        // 扣减库存
        await _inventoryService.DeductStockAsync(eventData.OrderId);
    }
}
```

---

## 贡献示例

欢迎贡献更多示例！请参阅 [贡献指南](../CONTRIBUTING.md)。

### 贡献步骤

1. Fork 本仓库
2. 在 `samples/` 目录下创建新的示例项目
3. 确保示例代码可以正常运行
4. 添加必要的 README 说明
5. 提交 Pull Request

### 示例要求

- 代码清晰、注释完整
- 遵循 Skywalker 框架的最佳实践
- 包含必要的配置说明
- 提供运行步骤

---

## 更多资源

- [使用指南](../docs/guide/README.md) - 详细的使用教程
- [API 文档](../docs/api/README.md) - 完整的 API 参考
- [架构设计](../docs/architecture/README.md) - 框架架构详解
