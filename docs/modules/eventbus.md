# 事件总线模块

本文档详细介绍 Skywalker 框架的事件总线模块。

---

## 目录

1. [模块概述](#模块概述)
2. [Skywalker.EventBus.Abstractions](#skywalkereventbusabstractions)
3. [Skywalker.EventBus.Local](#skywalkereventbuslocal)
4. [Skywalker.EventBus.RabbitMQ](#skywalkereventbusrabbitmq)

---

## 模块概述

| 模块 | NuGet 包 | 说明 |
|------|----------|------|
| Skywalker.EventBus.Abstractions | `Skywalker.EventBus.Abstractions` | 事件总线抽象接口 |
| Skywalker.EventBus.Local | `Skywalker.EventBus.Local` | 本地事件总线（进程内） |
| Skywalker.EventBus.RabbitMQ | `Skywalker.EventBus.RabbitMQ` | 分布式事件总线 |

### 依赖关系

```
Skywalker.EventBus.Local
└── Skywalker.EventBus.Abstractions

Skywalker.EventBus.RabbitMQ
├── Skywalker.EventBus.Abstractions
├── Skywalker.Extensions.RabbitMQ
└── RabbitMQ.Client
```

---

## Skywalker.EventBus.Abstractions

### 简介

事件总线抽象模块，定义事件总线的核心接口和基类。

### 安装

```bash
dotnet add package Skywalker.EventBus.Abstractions
```

### 核心类型

#### IEventBus - 事件总线接口

```csharp
namespace Skywalker.EventBus.Abstractions;

public interface IEventBus
{
    /// <summary>
    /// 发布事件
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <param name="eventArgs">事件数据</param>
    Task PublishAsync<TEvent>(TEvent eventArgs) where TEvent : class;
    
    /// <summary>
    /// 发布事件（非泛型）
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="eventArgs">事件数据</param>
    Task PublishAsync(Type eventType, object eventArgs);
}
```

#### IEventHandler - 事件处理器接口

```csharp
namespace Skywalker.EventBus.Abstractions;

/// <summary>
/// 事件处理器标记接口
/// </summary>
public interface IEventHandler
{
}

/// <summary>
/// 泛型事件处理器接口
/// </summary>
/// <typeparam name="TEvent">事件类型</typeparam>
public interface IEventHandler<in TEvent> : IEventHandler
{
    /// <summary>
    /// 处理事件
    /// </summary>
    /// <param name="eventData">事件数据</param>
    Task HandleEventAsync(TEvent eventData);
}
```

#### EventBusOptions - 事件总线选项

```csharp
namespace Skywalker.EventBus;

public sealed class EventBusOptions
{
    internal TypeList<IEventHandler> Handlers { get; set; } = [];
    
    /// <summary>
    /// 添加事件处理器
    /// </summary>
    public void AddEventHandler<THandler>() where THandler : IEventHandler
    {
        // 验证处理器必须实现 IEventHandler<TEvent>
        Handlers.Add<THandler>();
    }
}
```

### 定义事件

```csharp
// 定义订单创建事件
public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public string OrderNo { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}

// 定义订单确认事件
public class OrderConfirmedEvent
{
    public Guid OrderId { get; set; }
    public DateTime ConfirmedAt { get; set; }
}
```

### 定义事件处理器

```csharp
// 订单创建事件处理器
public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
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
        _logger.LogInformation("订单 {OrderNo} 已创建，金额: {Amount}", 
            eventData.OrderNo, eventData.TotalAmount);
        
        // 发送订单确认邮件
        await _emailService.SendOrderConfirmationAsync(
            eventData.OrderId, 
            eventData.CustomerId);
    }
}

// 库存扣减处理器
public class InventoryDeductionHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly IInventoryService _inventoryService;
    
    public InventoryDeductionHandler(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }
    
    public async Task HandleEventAsync(OrderCreatedEvent eventData)
    {
        await _inventoryService.DeductInventoryAsync(eventData.OrderId);
    }
}
```

---

## Skywalker.EventBus.Local

### 简介

本地事件总线实现，基于 `System.Threading.Channels` 实现高性能的进程内事件发布/订阅。

### 安装

```bash
dotnet add package Skywalker.EventBus.Local
```

### 核心类型

#### ILocalEventBus - 本地事件总线接口

```csharp
namespace Skywalker.EventBus.Local;

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

#### LocalChannelEventBus - 本地事件总线实现

```csharp
namespace Skywalker.EventBus.Local;

/// <summary>
/// 基于 System.Threading.Channels 的本地事件总线实现
/// </summary>
public class LocalChannelEventBus : EventBusBase, ILocalEventBus, IAsyncDisposable
{
    private readonly IEventHandlerFactory _handlerFactory;
    private readonly IEventHandlerInvoker _handlerInvoker;
    private readonly ConcurrentDictionary<Type, List<Type>> _handlerTypes;
    private readonly Channel<EventMessage> _channel;
    private readonly Task _processingTask;
    private readonly CancellationTokenSource _cts;

    public LocalChannelEventBus(
        IEventHandlerFactory handlerFactory,
        IEventHandlerInvoker handlerInvoker,
        IOptions<LocalEventBusOptions> options)
    {
        _handlerFactory = handlerFactory;
        _handlerInvoker = handlerInvoker;
        _handlerTypes = new ConcurrentDictionary<Type, List<Type>>();
        _cts = new CancellationTokenSource();

        // 创建有界通道
        _channel = Channel.CreateBounded<EventMessage>(new BoundedChannelOptions(options.Value.ChannelCapacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        });

        // 注册配置的处理器
        foreach (var handlerType in options.Value.Handlers)
        {
            RegisterHandler(handlerType);
        }

        // 启动后台处理任务
        _processingTask = StartProcessingAsync(_cts.Token);
    }

    public override async Task PublishAsync(Type eventType, object eventArgs)
    {
        await _channel.Writer.WriteAsync(new EventMessage(eventType, eventArgs));
    }

    public void Subscribe<TEvent, THandler>()
        where TEvent : class
        where THandler : IEventHandler<TEvent>
    {
        Subscribe(typeof(TEvent), typeof(THandler));
    }

    public void Subscribe(Type eventType, Type handlerType)
    {
        var handlers = _handlerTypes.GetOrAdd(eventType, _ => new List<Type>());
        lock (handlers)
        {
            if (!handlers.Contains(handlerType))
            {
                handlers.Add(handlerType);
            }
        }
    }

    public void Unsubscribe<TEvent, THandler>()
        where TEvent : class
        where THandler : IEventHandler<TEvent>
    {
        Unsubscribe(typeof(TEvent), typeof(THandler));
    }

    public void Unsubscribe(Type eventType, Type handlerType)
    {
        if (_handlerTypes.TryGetValue(eventType, out var handlers))
        {
            lock (handlers)
            {
                handlers.Remove(handlerType);
            }
        }
    }

    private async Task StartProcessingAsync(CancellationToken cancellationToken)
    {
        await foreach (var message in _channel.Reader.ReadAllAsync(cancellationToken))
        {
            var handlers = GetHandlers(message.EventType);
            foreach (var handlerType in handlers)
            {
                var handler = _handlerFactory.GetHandler(handlerType);
                await _handlerInvoker.InvokeAsync(handler, message.EventData, message.EventType);
            }
        }
    }
}
```

#### LocalEventBusOptions - 本地事件总线选项

```csharp
namespace Skywalker.EventBus.Local;

public class LocalEventBusOptions : EventBusOptions
{
    /// <summary>
    /// Channel 容量（默认 1000）
    /// </summary>
    public int ChannelCapacity { get; set; } = 1000;
}
```

### 注册服务

```csharp
// 注册本地事件总线
builder.Services.AddEventBusLocal();
```

> **说明**：事件处理器通过实现 `ILocalEventHandler<TEvent>` 接口并通过 SourceGenerator 自动注册，无需手动添加。

### 使用示例

#### 发布事件

```csharp
public class OrderAppService : ApplicationService
{
    private readonly ILocalEventBus _eventBus;
    private readonly IRepository<Order, Guid> _orderRepository;

    public OrderAppService(
        IMapper mapper,
        ILocalEventBus eventBus,
        IRepository<Order, Guid> orderRepository) : base(mapper)
    {
        _eventBus = eventBus;
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto input)
    {
        var order = new Order(Guid.NewGuid(), GenerateOrderNo(), input.CustomerId);

        foreach (var item in input.Items)
        {
            order.AddItem(item.ProductId, item.ProductName, item.Price, item.Quantity);
        }

        await _orderRepository.InsertAsync(order, autoSave: true);

        // 发布订单创建事件
        await _eventBus.PublishAsync(new OrderCreatedEvent
        {
            OrderId = order.Id,
            OrderNo = order.OrderNo,
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount,
            CreatedAt = DateTime.UtcNow
        });

        return Mapper.Map<OrderDto>(order);
    }

    public async Task ConfirmOrderAsync(Guid orderId)
    {
        var order = await _orderRepository.GetAsync(orderId);
        order.Confirm();
        await _orderRepository.UpdateAsync(order, autoSave: true);

        // 发布订单确认事件
        await _eventBus.PublishAsync(new OrderConfirmedEvent
        {
            OrderId = orderId,
            ConfirmedAt = DateTime.UtcNow
        });
    }
}
```

#### 动态订阅/取消订阅

```csharp
public class EventSubscriptionService
{
    private readonly ILocalEventBus _eventBus;

    public EventSubscriptionService(ILocalEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public void SubscribeToOrderEvents()
    {
        // 动态订阅
        _eventBus.Subscribe<OrderCreatedEvent, OrderCreatedEventHandler>();
        _eventBus.Subscribe<OrderConfirmedEvent, OrderConfirmedEventHandler>();
    }

    public void UnsubscribeFromOrderEvents()
    {
        // 动态取消订阅
        _eventBus.Unsubscribe<OrderCreatedEvent, OrderCreatedEventHandler>();
        _eventBus.Unsubscribe<OrderConfirmedEvent, OrderConfirmedEventHandler>();
    }
}
```

---

## Skywalker.EventBus.RabbitMQ

### 简介

基于 RabbitMQ 的分布式事件总线实现，支持跨进程、跨服务的事件发布/订阅。

### 安装

```bash
dotnet add package Skywalker.EventBus.RabbitMQ
```

### 核心类型

#### RabbitMqEventBus - RabbitMQ 事件总线

```csharp
namespace Skywalker.EventBus.RabbitMQ;

public class RabbitMqEventBus : EventBusBase
{
    protected RabbitMqEventBusOptions RabbitMqEventBusOptions { get; }
    protected IConnectionPool ConnectionPool { get; }
    protected IRabbitMqSerializer Serializer { get; }
    protected IGuidGenerator GuidGenerator { get; }
    protected ILogger<RabbitMqEventBus> Logger { get; }

    public override Task PublishAsync(Type eventType, object eventArgs)
    {
        var routingKey = EventNameAttribute.GetNameOrDefault(eventType);
        var body = Serializer.Serialize(eventArgs);
        return PublishAsync(routingKey, body);
    }

    public virtual Task PublishAsync(
        Type eventType,
        object eventData,
        Dictionary<string, object>? headersArguments = null,
        Guid? eventId = null,
        string? correlationId = null)
    {
        var routingKey = EventNameAttribute.GetNameOrDefault(eventType);
        var body = Serializer.Serialize(eventData);
        return PublishAsync(routingKey, body, headersArguments, eventId, correlationId);
    }
}
```

#### RabbitMqEventBusOptions - RabbitMQ 事件总线选项

```csharp
namespace Skywalker.EventBus.RabbitMQ;

public class RabbitMqEventBusOptions : EventBusOptions
{
    /// <summary>
    /// 连接名称
    /// </summary>
    public string ConnectionName { get; set; } = "Default";

    /// <summary>
    /// Exchange 名称
    /// </summary>
    public string ExchangeName { get; set; } = "skywalker.events";

    /// <summary>
    /// Exchange 类型（默认 topic）
    /// </summary>
    public string ExchangeType { get; set; } = "topic";

    /// <summary>
    /// 队列名称前缀
    /// </summary>
    public string QueueNamePrefix { get; set; } = "";
}
```

#### EventNameAttribute - 事件名称特性

```csharp
namespace Skywalker.EventBus;

[AttributeUsage(AttributeTargets.Class)]
public class EventNameAttribute : Attribute
{
    public string Name { get; }

    public EventNameAttribute(string name)
    {
        Name = name;
    }

    public static string GetNameOrDefault(Type eventType)
    {
        var attribute = eventType.GetCustomAttribute<EventNameAttribute>();
        return attribute?.Name ?? eventType.FullName!;
    }
}
```

### 配置

#### appsettings.json

```json
{
  "RabbitMQ": {
    "Connections": {
      "Default": {
        "HostName": "localhost",
        "Port": 5672,
        "UserName": "guest",
        "Password": "guest",
        "VirtualHost": "/"
      }
    },
    "EventBus": {
      "ExchangeName": "skywalker.events",
      "ExchangeType": "topic",
      "QueueNamePrefix": "order-service"
    }
  }
}
```

### 注册服务

```csharp
// 注册 RabbitMQ 事件总线
builder.Services.AddEventBusRabbitMQ();
```

> **说明**：RabbitMQ 连接配置通过 `appsettings.json` 的 `RabbitMQ` 节点酋入，事件处理器通过 SourceGenerator 自动注册。

### 使用示例

#### 定义分布式事件

```csharp
// 使用 EventNameAttribute 指定路由键
[EventName("order.created")]
public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public string OrderNo { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}

[EventName("payment.completed")]
public class PaymentCompletedEvent
{
    public Guid PaymentId { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = null!;
    public DateTime CompletedAt { get; set; }
}
```

#### 发布分布式事件

```csharp
public class OrderAppService : ApplicationService
{
    private readonly IEventBus _eventBus;

    public OrderAppService(IMapper mapper, IEventBus eventBus) : base(mapper)
    {
        _eventBus = eventBus;
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto input)
    {
        // 创建订单...

        // 发布分布式事件
        await _eventBus.PublishAsync(new OrderCreatedEvent
        {
            OrderId = order.Id,
            OrderNo = order.OrderNo,
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount,
            CreatedAt = DateTime.UtcNow
        });

        return Mapper.Map<OrderDto>(order);
    }
}
```

#### 处理分布式事件

```csharp
// 在支付服务中处理订单创建事件
public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;
    private readonly IPaymentService _paymentService;

    public OrderCreatedEventHandler(
        ILogger<OrderCreatedEventHandler> logger,
        IPaymentService paymentService)
    {
        _logger = logger;
        _paymentService = paymentService;
    }

    public async Task HandleEventAsync(OrderCreatedEvent eventData)
    {
        _logger.LogInformation("收到订单创建事件: {OrderNo}", eventData.OrderNo);

        // 创建待支付记录
        await _paymentService.CreatePendingPaymentAsync(
            eventData.OrderId,
            eventData.TotalAmount);
    }
}
```

---

## 最佳实践

### 1. 事件设计

```csharp
// ✅ 好的实践：事件包含足够的上下文信息
public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public string OrderNo { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemInfo> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

// ❌ 不好的实践：事件信息不足，需要额外查询
public class OrderCreatedEvent
{
    public Guid OrderId { get; set; } // 只有 ID，处理器需要查询数据库
}
```

### 2. 处理器设计

```csharp
// ✅ 好的实践：处理器职责单一
public class SendOrderConfirmationEmailHandler : IEventHandler<OrderCreatedEvent>
{
    public async Task HandleEventAsync(OrderCreatedEvent eventData)
    {
        // 只负责发送邮件
    }
}

public class DeductInventoryHandler : IEventHandler<OrderCreatedEvent>
{
    public async Task HandleEventAsync(OrderCreatedEvent eventData)
    {
        // 只负责扣减库存
    }
}

// ❌ 不好的实践：处理器做太多事情
public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    public async Task HandleEventAsync(OrderCreatedEvent eventData)
    {
        // 发送邮件
        // 扣减库存
        // 更新统计
        // 通知仓库
        // ...
    }
}
```

### 3. 错误处理

```csharp
public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public async Task HandleEventAsync(OrderCreatedEvent eventData)
    {
        try
        {
            // 处理逻辑
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理订单创建事件失败: {OrderId}", eventData.OrderId);

            // 根据业务需求决定是否重新抛出异常
            // 对于分布式事件，可能需要重试机制
            throw;
        }
    }
}
```

### 4. 本地 vs 分布式事件

| 场景 | 推荐 |
|------|------|
| 同一进程内的模块解耦 | 本地事件总线 |
| 跨服务通信 | 分布式事件总线 |
| 需要持久化的事件 | 分布式事件总线 |
| 高性能、低延迟 | 本地事件总线 |
| 需要重试机制 | 分布式事件总线 |

---

## 相关文档

- [使用指南 - 事件总线](../guide/README.md#事件总线)
- [API 文档 - 事件总线 API](../api/README.md#事件总线-api)
- [架构设计](../architecture/README.md)
```

