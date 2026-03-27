# Skywalker 架构设计文档

本文档详细介绍 Skywalker 框架的架构设计、核心概念、设计决策和扩展指南。

---

## 目录

1. [架构概述](#架构概述)
2. [分层架构](#分层架构)
3. [模块依赖关系](#模块依赖关系)
4. [核心概念](#核心概念)
5. [基础设施](#基础设施)
6. [设计决策](#设计决策)
7. [扩展指南](#扩展指南)
8. [最佳实践](#最佳实践)

---

## 架构概述

Skywalker 是一个基于 **领域驱动设计（DDD）** 原则构建的模块化 .NET 应用开发框架。

### 设计目标

| 目标 | 说明 |
|------|------|
| **模块化** | 高度模块化设计，40+ 独立模块，按需引用 |
| **松耦合** | 通过依赖注入和事件总线实现组件间松耦合 |
| **可扩展** | 提供丰富的扩展点，支持自定义实现 |
| **可测试** | 面向接口编程，便于单元测试和集成测试 |
| **高性能** | 合理的抽象层次，避免过度封装带来的性能损耗 |

### 技术选型

| 技术领域 | 选择 | 理由 |
|----------|------|------|
| **运行时** | .NET 8.0 | 最新 LTS 版本，性能优异 |
| **ORM** | Entity Framework Core 8.0 | 成熟稳定，LINQ 支持好 |
| **依赖注入** | Microsoft.Extensions.DependencyInjection | .NET 官方 DI 容器 |
| **序列化** | System.Text.Json | 高性能 JSON 序列化 |
| **消息队列** | RabbitMQ | 成熟稳定的消息中间件 |
| **缓存** | Redis + MemoryCache | 多级缓存策略 |
| **模板引擎** | Scriban | 高性能、安全的模板引擎 |

---

## 分层架构

Skywalker 采用经典的 **四层架构**，遵循 DDD 分层原则。

### 架构图

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                            Presentation Layer                                │
│    ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐       │
│    │  REST API   │  │    gRPC     │  │   GraphQL   │  │    MVC      │       │
│    └─────────────┘  └─────────────┘  └─────────────┘  └─────────────┘       │
├─────────────────────────────────────────────────────────────────────────────┤
│                            Application Layer                                 │
│    ┌─────────────────────────────────────────────────────────────────┐      │
│    │  Application Services  │  DTOs  │  Validators  │  Event Handlers│      │
│    └─────────────────────────────────────────────────────────────────┘      │
├─────────────────────────────────────────────────────────────────────────────┤
│                              Domain Layer                                    │
│    ┌───────────────────────────────────────────────────────────────────┐    │
│    │  Aggregates  │  Entities  │  Value Objects  │  Domain Services    │    │
│    │  Repositories (Interface)  │  Domain Events  │  Specifications    │    │
│    └───────────────────────────────────────────────────────────────────┘    │
├─────────────────────────────────────────────────────────────────────────────┤
│                           Infrastructure Layer                               │
│    ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐       │
│    │   EF Core   │  │   Caching   │  │  Event Bus  │  │   外部服务   │       │
│    │  (MySQL/    │  │   (Redis/   │  │ (RabbitMQ/  │  │  (邮件/短信  │       │
│    │  SqlServer) │  │   Memory)   │  │   Local)    │  │   /支付)    │       │
│    └─────────────┘  └─────────────┘  └─────────────┘  └─────────────┘       │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 各层职责

#### 1. 表现层 (Presentation Layer)

**职责**：处理用户请求，返回响应

| 组件 | 职责 |
|------|------|
| Controllers | 接收 HTTP 请求，调用应用服务，返回响应 |
| Filters | 请求/响应过滤（认证、授权、异常处理） |
| ViewModels | 视图模型（MVC 场景） |

**依赖**：Application Layer

```csharp
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderAppService _orderAppService;

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> Get(Guid id)
    {
        var order = await _orderAppService.GetAsync(id);
        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderInput input)
    {
        var order = await _orderAppService.CreateAsync(input);
        return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
    }
}
```

#### 2. 应用层 (Application Layer)

**职责**：协调领域对象完成用例，不包含业务规则

| 组件 | 职责 |
|------|------|
| Application Services | 编排用例流程，调用领域服务和仓储 |
| DTOs | 数据传输对象，用于层间数据传递 |
| Validators | 输入验证 |
| Event Handlers | 事件处理器 |
| Object Mapping | DTO 与实体之间的映射 |

**依赖**：Domain Layer

```csharp
public class OrderAppService : ApplicationService, IOrderAppService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderDomainService _orderDomainService;
    private readonly ILocalEventBus _eventBus;

    public async Task<OrderDto> CreateAsync(CreateOrderInput input)
    {
        // 1. 调用领域服务创建订单
        var order = await _orderDomainService.CreateOrderAsync(
            input.CustomerId,
            input.ShippingAddress,
            input.Items);

        // 2. 持久化
        await _orderRepository.InsertAsync(order);

        // 3. 发布事件
        await _eventBus.PublishAsync(new OrderCreatedEvent { OrderId = order.Id });

        // 4. 映射并返回
        return ObjectMapper.Map<Order, OrderDto>(order);
    }
}
```

#### 3. 领域层 (Domain Layer)

**职责**：包含核心业务逻辑和规则

| 组件 | 职责 |
|------|------|
| Entities | 具有唯一标识的领域对象 |
| Aggregate Roots | 聚合的入口点，维护聚合一致性 |
| Value Objects | 通过属性值区分的不可变对象 |
| Domain Services | 不属于单一实体的领域逻辑 |
| Repository Interfaces | 仓储接口定义（不含实现） |
| Domain Events | 领域事件定义 |
| Specifications | 查询规约 |

**依赖**：无（最内层）

```csharp
// 聚合根 - 包含业务规则
public class Order : AggregateRoot<Guid>
{
    public void AddItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        // 业务规则：只有待处理状态才能添加商品
        if (Status != OrderStatus.Pending)
            throw new BusinessException("ORDER_INVALID_STATUS", "只有待处理订单可以添加商品");

        // 业务规则：同一商品合并数量
        var existingItem = _items.FirstOrDefault(x => x.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.ChangeQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            _items.Add(new OrderItem(...));
        }

        RecalculateTotal();
    }
}
```

#### 4. 基础设施层 (Infrastructure Layer)

**职责**：提供技术实现

| 组件 | 职责 |
|------|------|
| Repository Implementations | 仓储接口的具体实现 |
| DbContext | 数据库上下文配置 |
| External Service Clients | 外部服务客户端（邮件、短信、支付等） |
| Caching Implementation | 缓存实现 |
| Messaging Implementation | 消息队列实现 |

**依赖**：Domain Layer（实现其定义的接口）

```csharp
// 仓储实现
public class OrderRepository : EfCoreRepository<AppDbContext, Order, Guid>, IOrderRepository
{
    public async Task<Order?> GetByOrderNoAsync(string orderNo, CancellationToken ct = default)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderNo == orderNo, ct);
    }
}
```

### 依赖规则

```
                    ┌──────────────────┐
                    │  Presentation    │
                    └────────┬─────────┘
                             │ 依赖
                             ▼
                    ┌──────────────────┐
                    │   Application    │
                    └────────┬─────────┘
                             │ 依赖
                             ▼
    ┌──────────────┐    ┌──────────────────┐
    │Infrastructure│───▶│     Domain       │
    └──────────────┘    └──────────────────┘
           │ 实现                ▲
           └────────────────────┘
```

**核心原则**：
- 内层不依赖外层
- Domain 层是最内层，不依赖任何其他层
- Infrastructure 依赖 Domain（实现其接口）
- Application 依赖 Domain（使用其模型和接口）
- Presentation 依赖 Application（调用其服务）

---

## 模块依赖关系

### 完整模块依赖图

```
                           ┌─────────────────────────┐
                           │    Skywalker.Ddd        │
                           │      (整合包)           │
                           └───────────┬─────────────┘
                                       │
           ┌───────────────────────────┼───────────────────────────┐
           │                           │                           │
           ▼                           ▼                           ▼
┌─────────────────────┐   ┌─────────────────────┐   ┌─────────────────────┐
│ Skywalker.Ddd.Domain │   │Skywalker.Ddd.Application│  │ Skywalker.Ddd.Uow   │
└─────────┬───────────┘   └─────────┬───────────┘   └─────────┬───────────┘
          │                         │                         │
          ▼                         ▼                         ▼
┌─────────────────────┐   ┌─────────────────────┐   ┌─────────────────────┐
│Skywalker.Ddd.Domain │   │Skywalker.Ddd.App    │   │Skywalker.Extensions │
│   .Abstractions     │   │   .Abstractions     │   │     .Universal      │
└─────────────────────┘   └─────────────────────┘   └─────────────────────┘

                     ┌─────────────────────────────────┐
                     │Skywalker.Ddd.EntityFrameworkCore │
                     └───────────────┬─────────────────┘
                                     │
                 ┌───────────────────┼───────────────────┐
                 │                   │                   │
                 ▼                   ▼                   ▼
    ┌────────────────────┐ ┌────────────────┐ ┌────────────────────┐
    │Skywalker.Ddd.Domain│ │Skywalker.Ddd   │ │ Microsoft.Entity   │
    └────────────────────┘ │     .Uow       │ │   FrameworkCore    │
                           └────────────────┘ └────────────────────┘

         ┌────────────────────────────────────────────────────┐
         │              Event Bus Modules                     │
         └────────────────────────────────────────────────────┘

┌─────────────────────────┐    ┌─────────────────────────┐
│ Skywalker.EventBus.Local │    │Skywalker.EventBus.RabbitMQ│
└───────────┬─────────────┘    └───────────┬─────────────┘
            │                              │
            ▼                              ▼
    ┌───────────────────────────────────────────────┐
    │      Skywalker.EventBus.Abstractions          │
    └───────────────────────────────────────────────┘

         ┌────────────────────────────────────────────────────┐
         │                Caching Modules                     │
         └────────────────────────────────────────────────────┘

┌──────────────────────────┐   ┌──────────────────────────┐
│ Skywalker.Caching.Memory │   │ Skywalker.Caching.Redis  │
└───────────┬──────────────┘   └───────────┬──────────────┘
            │                              │
            ▼                              ▼
    ┌───────────────────────────────────────────────┐
    │      Skywalker.Caching.Abstractions           │
    └───────────────────────────────────────────────┘
```

### 模块分类

| 分类 | 模块 | 说明 |
|------|------|------|
| **核心模块** | Skywalker.Ddd | DDD 整合包 |
| | Skywalker.Ddd.Domain | 领域层基础设施 |
| | Skywalker.Ddd.Application | 应用层基础设施 |
| | Skywalker.Ddd.Uow | 工作单元 |
| **数据访问** | Skywalker.Ddd.EntityFrameworkCore | EF Core 集成 |
| | Skywalker.Ddd.EntityFrameworkCore.MySQL | MySQL 支持 |
| | Skywalker.Ddd.EntityFrameworkCore.SqlServer | SQL Server 支持 |
| **事件总线** | Skywalker.EventBus.Abstractions | 事件总线抽象 |
| | Skywalker.EventBus.Local | 本地事件总线 |
| | Skywalker.EventBus.RabbitMQ | RabbitMQ 事件总线 |
| **缓存** | Skywalker.Caching.Abstractions | 缓存抽象 |
| | Skywalker.Caching.Memory | 内存缓存 |
| | Skywalker.Caching.Redis | Redis 缓存 |
| **其他** | Skywalker.Settings.Abstractions | 设置管理 |
| | Skywalker.Permissions.Abstractions | 权限管理 |
| | Skywalker.Localization.Abstractions | 本地化 |
| | Skywalker.Validation.Abstractions | 数据验证 |
| | Skywalker.ObjectMapping.Abstractions | 对象映射 |
| | Skywalker.Template.Abstractions | 模板引擎 |

---

## 核心概念

### 实体 (Entity)

实体是具有唯一标识的领域对象，其生命周期中标识保持不变。

#### 特性

- **唯一标识**：通过 Id 属性区分实体
- **可变性**：实体的属性可以改变
- **生命周期**：实体有创建、修改、删除的生命周期
- **乐观并发**：通过 ConcurrencyStamp 实现

#### 类图

```
┌────────────────────────────────────────┐
│              Entity<TKey>              │
├────────────────────────────────────────┤
│ + Id: TKey                             │
│ + ConcurrencyStamp: string?            │
│ + CreationTime: DateTime               │
├────────────────────────────────────────┤
│ + GetKeys(): object[]                  │
│ + EntityEquals(IEntity): bool          │
└────────────────────────────────────────┘
```

### 聚合根 (Aggregate Root)

聚合根是聚合的入口点，负责维护聚合内部的一致性。

#### 设计原则

1. **边界清晰**：聚合定义了一组相关对象的边界
2. **一致性边界**：聚合内部保持强一致性
3. **事务边界**：一个事务只修改一个聚合
4. **单一入口**：只能通过聚合根访问聚合内的实体
5. **领域事件**：聚合根可以发布领域事件

#### 示例

```
                    ┌──────────────────────────┐
                    │    Order (聚合根)         │
                    │  ┌────────────────────┐  │
                    │  │ - OrderNo          │  │
                    │  │ - CustomerId       │  │
                    │  │ - Status           │  │
                    │  │ - TotalAmount      │  │
                    │  └────────────────────┘  │
                    │           │              │
                    │     ┌─────┴─────┐        │
                    │     ▼           ▼        │
                    │ ┌────────┐ ┌────────┐   │
                    │ │OrderItem│ │OrderItem│   │
                    │ └────────┘ └────────┘   │
                    │                          │
                    │ ┌────────────────────┐  │
                    │ │ Address (值对象)    │  │
                    │ └────────────────────┘  │
                    └──────────────────────────┘
```

### 值对象 (Value Object)

值对象没有标识，通过属性值来区分，是不可变的。

#### 特性

- **无标识**：通过属性值而非 Id 区分
- **不可变**：创建后不能修改
- **可替换**：可以用另一个值对象完全替换
- **相等性**：通过属性值比较相等性

#### 常见值对象

| 值对象 | 属性 |
|--------|------|
| Address | Province, City, District, Street, PostalCode |
| Money | Amount, Currency |
| DateRange | StartDate, EndDate |
| EmailAddress | Value |
| PhoneNumber | CountryCode, Number |

### 仓储 (Repository)

仓储提供对聚合根的持久化访问，隐藏数据访问细节。

#### 设计原则

1. **只为聚合根创建仓储**
2. **返回完整的聚合**（包含关联实体）
3. **隐藏持久化细节**
4. **支持规约模式**

#### 仓储接口层次

```
        ┌─────────────────────────────────────────┐
        │         IRepository<TEntity, TKey>       │
        └────────────────────┬────────────────────┘
                             │ 继承
        ┌────────────────────┼────────────────────┐
        │                    │                    │
        ▼                    ▼                    ▼
┌───────────────┐   ┌───────────────┐   ┌───────────────┐
│IReadOnlyRepo  │   │ IBasicRepo    │   │ IRepository   │
│<TEntity,TKey> │   │<TEntity,TKey> │   │ <TEntity>     │
└───────────────┘   └───────────────┘   └───────────────┘
    │                    │
    │ 查询方法            │ CRUD 方法
    │ - FindAsync        │ - InsertAsync
    │ - GetAsync         │ - UpdateAsync
    │ - GetListAsync     │ - DeleteAsync
    │ - GetPagedListAsync│
```

### 领域服务 (Domain Service)

领域服务封装不属于任何单一实体的领域逻辑。

#### 使用场景

1. **跨聚合操作**：涉及多个聚合的业务逻辑
2. **复杂业务规则**：不适合放在实体中的复杂规则
3. **外部依赖**：需要访问外部服务的领域逻辑

### 领域事件 (Domain Event)

领域事件用于在聚合之间传递信息，实现松耦合。

#### 事件流程

```
┌─────────────┐    AddDistributedEvent()    ┌─────────────┐
│  Aggregate  │ ─────────────────────────▶ │ Event List  │
└─────────────┘                            └──────┬──────┘
                                                  │
                                           SaveChangesAsync()
                                                  │
                                                  ▼
                                           ┌─────────────┐
                                           │  Event Bus  │
                                           └──────┬──────┘
                                                  │
                              ┌───────────────────┼───────────────────┐
                              │                   │                   │
                              ▼                   ▼                   ▼
                        ┌───────────┐       ┌───────────┐       ┌───────────┐
                        │ Handler 1 │       │ Handler 2 │       │ Handler 3 │
                        └───────────┘       └───────────┘       └───────────┘
```

---

## 基础设施

### 工作单元 (Unit of Work)

工作单元模式确保多个仓储操作在同一事务中执行。

#### 工作流程

```
┌─────────────────────────────────────────────────────────────┐
│                     Unit of Work                            │
│  ┌─────────────────────────────────────────────────────┐   │
│  │                  Transaction                         │   │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  │   │
│  │  │ Repository1 │  │ Repository2 │  │ Repository3 │  │   │
│  │  │  Insert     │  │   Update    │  │   Delete    │  │   │
│  │  └─────────────┘  └─────────────┘  └─────────────┘  │   │
│  └─────────────────────────────────────────────────────┘   │
│                           │                                 │
│                           ▼                                 │
│                    CompleteAsync()                          │
│                           │                                 │
│              ┌────────────┴────────────┐                   │
│              ▼                         ▼                    │
│         ✅ Commit                  ❌ Rollback              │
└─────────────────────────────────────────────────────────────┘
```

### 事件总线 (Event Bus)

#### 本地事件总线架构

```
                      ┌────────────────────────────────────┐
                      │      LocalChannelEventBus          │
                      └──────────────────┬─────────────────┘
                                         │
                      ┌──────────────────┼──────────────────┐
                      │                  │                  │
                      ▼                  ▼                  ▼
              ┌───────────────┐  ┌───────────────┐  ┌───────────────┐
              │   Publisher   │  │    Channel    │  │   Consumer    │
              │               │  │   (Bounded)   │  │   (Task)      │
              │ PublishAsync()│─▶│  ───────────  │─▶│ ReadAllAsync()│
              └───────────────┘  └───────────────┘  └───────────────┘
                                                           │
                                                           ▼
                                         ┌─────────────────────────────┐
                                         │     Handler Factory         │
                                         │  ┌───────────────────────┐  │
                                         │  │  IEventHandler<TEvent>│  │
                                         │  └───────────────────────┘  │
                                         └─────────────────────────────┘
```

#### 分布式事件总线架构

```
┌─────────────────┐         ┌─────────────────┐         ┌─────────────────┐
│   Service A     │         │    RabbitMQ     │         │   Service B     │
│                 │         │                 │         │                 │
│  ┌───────────┐  │  Pub    │  ┌───────────┐  │  Sub    │  ┌───────────┐  │
│  │ EventBus  │──┼────────▶│  │  Exchange │──┼────────▶│  │ EventBus  │  │
│  └───────────┘  │         │  └─────┬─────┘  │         │  └───────────┘  │
└─────────────────┘         │        │        │         └─────────────────┘
                            │  ┌─────▼─────┐  │
                            │  │   Queue   │  │
                            │  └───────────┘  │
                            └─────────────────┘
```

### 缓存策略

#### 多级缓存

```
┌─────────────────────────────────────────────────────────────────┐
│                        Application                               │
└─────────────────────────────────────────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────────┐
│                     L1: Memory Cache                            │
│                   (IMemoryCache)                                │
│                   - 超快访问速度                                 │
│                   - 进程内有效                                   │
│                   - 容量有限                                     │
└─────────────────────────────────────────────────────────────────┘
                               │ Miss
                               ▼
┌─────────────────────────────────────────────────────────────────┐
│                     L2: Redis Cache                             │
│                   (StackExchange.Redis)                         │
│                   - 分布式共享                                   │
│                   - 持久化支持                                   │
│                   - 大容量                                       │
└─────────────────────────────────────────────────────────────────┘
                               │ Miss
                               ▼
┌─────────────────────────────────────────────────────────────────┐
│                        Database                                  │
└─────────────────────────────────────────────────────────────────┘
```

---

## 设计决策

### 为什么选择 Entity Framework Core？

| 优点 | 说明 |
|------|------|
| **成熟稳定** | 微软官方 ORM，社区活跃 |
| **LINQ 支持** | 类型安全的查询，编译时检查 |
| **多数据库** | 支持 SQL Server、MySQL、PostgreSQL、SQLite 等 |
| **迁移工具** | 内置数据库迁移支持 |
| **变更追踪** | 自动检测实体变化 |
| **延迟加载** | 按需加载关联数据 |

### 为什么采用模块化设计？

| 优点 | 说明 |
|------|------|
| **按需引用** | 只安装需要的模块，减小部署体积 |
| **松耦合** | 模块间通过接口通信，便于替换 |
| **可测试** | 模块独立，易于单元测试 |
| **可扩展** | 可以添加新模块扩展功能 |
| **关注点分离** | 每个模块专注于特定功能 |

### 为什么使用依赖注入？

| 优点 | 说明 |
|------|------|
| **解耦** | 组件不直接依赖具体实现 |
| **可测试** | 便于 Mock 依赖进行单元测试 |
| **灵活** | 运行时可替换实现 |
| **生命周期管理** | 自动管理对象生命周期 |
| **ASP.NET Core 原生** | 与 ASP.NET Core 深度集成 |

### 为什么使用 Channel 实现本地事件总线？

| 优点 | 说明 |
|------|------|
| **高性能** | .NET 原生异步集合，性能优异 |
| **背压处理** | BoundedChannel 自动处理背压 |
| **无锁设计** | 减少锁竞争，提高并发性能 |
| **简单可靠** | 无外部依赖，稳定可靠 |

---

## 扩展指南

### 自定义仓储实现

```csharp
// 1. 实现仓储基类
public class DapperRepository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{
    private readonly IDbConnection _connection;

    public async Task<TEntity?> FindAsync(TKey id, CancellationToken ct = default)
    {
        return await _connection.QueryFirstOrDefaultAsync<TEntity>(
            $"SELECT * FROM {typeof(TEntity).Name}s WHERE Id = @Id",
            new { Id = id });
    }

    // ... 其他方法实现
}

// 2. 注册仓储
services.AddTransient(typeof(IRepository<,>), typeof(DapperRepository<,>));
```

### 自定义缓存提供者

```csharp
// 1. 实现 ICaching 接口
public class MemcachedCaching : ICaching
{
    private readonly IMemcachedClient _client;

    public async ValueTask<TValue?> GetAsync<TValue>(string key, CancellationToken ct = default)
    {
        return await _client.GetAsync<TValue>(key);
    }

    public async Task SetAsync<TValue>(string key, TValue value, TimeSpan? expireTime = null, CancellationToken ct = default)
    {
        await _client.SetAsync(key, value, expireTime ?? DefaultExpireTime ?? TimeSpan.FromHours(1));
    }

    // ... 其他方法实现
}

// 2. 注册缓存提供者
services.AddSingleton<ICaching, MemcachedCaching>();
```

### 自定义事件处理器

```csharp
// 1. 定义事件
public class UserRegisteredEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
}

// 2. 实现处理器
public class SendWelcomeEmailHandler : ILocalEventHandler<UserRegisteredEvent>
{
    private readonly IEmailService _emailService;

    public async Task HandleEventAsync(UserRegisteredEvent eventData)
    {
        await _emailService.SendWelcomeEmailAsync(eventData.Email);
    }
}

// 3. 注册本地事件总线
builder.Services.AddEventBusLocal();
```

### 自定义设置提供者

```csharp
// 1. 实现 ISettingValueProvider
public class EnvironmentSettingValueProvider : ISettingValueProvider
{
    public string Name => "Environment";

    public Task<string?> GetOrNullAsync(SettingDefinition setting)
    {
        var envKey = $"SKYWALKER_{setting.Name.Replace(".", "_").ToUpperInvariant()}";
        return Task.FromResult(Environment.GetEnvironmentVariable(envKey));
    }
}

// 2. 注册提供者
services.AddSingleton<ISettingValueProvider, EnvironmentSettingValueProvider>();
```

---

## 最佳实践

### 聚合设计原则

| 原则 | 说明 | 示例 |
|------|------|------|
| **小而精** | 聚合应该尽可能小 | Order 聚合只包含 OrderItem，不包含 Product |
| **一致性边界** | 聚合内强一致，聚合间最终一致 | 订单创建和库存扣减可以分开事务 |
| **通过 ID 引用** | 聚合之间通过 ID 引用，不直接引用 | Order.CustomerId 而不是 Order.Customer |
| **单一聚合根** | 每个聚合只有一个聚合根 | Order 是聚合根，OrderItem 不是 |

### 仓储使用规范

```csharp
// ✅ 正确：只为聚合根创建仓储
public interface IOrderRepository : IRepository<Order, Guid> { }

// ❌ 错误：为非聚合根创建仓储
public interface IOrderItemRepository : IRepository<OrderItem, Guid> { }

// ✅ 正确：返回完整聚合
public async Task<Order> GetAsync(Guid id)
{
    return await dbContext.Orders
        .Include(o => o.Items)  // 包含子实体
        .FirstAsync(o => o.Id == id);
}

// ❌ 错误：返回不完整聚合
public async Task<Order> GetAsync(Guid id)
{
    return await dbContext.Orders.FindAsync(id);  // 缺少 Items
}
```

### 领域事件使用规范

```csharp
// ✅ 正确：在聚合根中添加事件
public class Order : AggregateRoot<Guid>
{
    public void Confirm()
    {
        Status = OrderStatus.Confirmed;
        AddDistributedEvent(new OrderConfirmedEvent { OrderId = Id });
    }
}

// ❌ 错误：在应用服务中发布应该由聚合发布的事件
public class OrderAppService
{
    public async Task ConfirmAsync(Guid id)
    {
        var order = await _repository.GetAsync(id);
        order.Status = OrderStatus.Confirmed;  // 直接修改状态
        await _eventBus.PublishAsync(new OrderConfirmedEvent { OrderId = id });  // 应该由聚合发布
    }
}
```

### 分层边界规范

```csharp
// ✅ 正确：应用层返回 DTO
public async Task<OrderDto> GetAsync(Guid id)
{
    var order = await _repository.GetAsync(id);
    return ObjectMapper.Map<Order, OrderDto>(order);
}

// ❌ 错误：应用层返回领域实体
public async Task<Order> GetAsync(Guid id)
{
    return await _repository.GetAsync(id);  // 暴露领域模型
}

// ✅ 正确：控制器调用应用服务
[HttpGet("{id}")]
public async Task<OrderDto> Get(Guid id)
{
    return await _orderAppService.GetAsync(id);
}

// ❌ 错误：控制器直接调用仓储
[HttpGet("{id}")]
public async Task<Order> Get(Guid id)
{
    return await _orderRepository.GetAsync(id);  // 跨层调用
}
```

---

## 更多资源

- [使用指南](../guide/README.md) - 详细的使用教程
- [API 文档](../api/README.md) - 完整的 API 参考
- [示例项目](../../samples/README.md) - 完整示例代码
- [贡献指南](../../CONTRIBUTING.md) - 如何参与贡献

