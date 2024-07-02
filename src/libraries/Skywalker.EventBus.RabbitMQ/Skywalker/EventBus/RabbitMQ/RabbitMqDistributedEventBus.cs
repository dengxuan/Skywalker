using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Skywalker.EventBus.Abstractions;
using Skywalker.Extensions.GuidGenerator;
using Skywalker.Extensions.RabbitMQ;
using Skywalker.Extensions.RabbitMQ.Abstractions;
using Skywalker.Extensions.Timezone;

namespace Skywalker.EventBus.RabbitMQ;

public class RabbitMqDistributedEventBus : EventBusBase
{
    protected RabbitMqEventBusOptions RabbitMqEventBusOptions { get; }
    protected IConnectionPool ConnectionPool { get; }
    protected IRabbitMqSerializer Serializer { get; }
    protected IGuidGenerator GuidGenerator { get; }

    protected ConcurrentDictionary<Type, List<IEventHandlerFactory>> HandlerFactories { get; }
    protected ConcurrentDictionary<string, Type> EventTypes { get; }
    protected IRabbitMqMessageConsumerFactory MessageConsumerFactory { get; }
    protected IRabbitMqMessageConsumer Consumer { get; private set; } = default!;
    protected ILogger<RabbitMqDistributedEventBus> Logger { get; }
    protected IEventHandlerInvoker EventHandlerInvoker { get; }
    protected EventBusBuilder EventBusOptions { get; }

    private bool _exchangeCreated;

    public RabbitMqDistributedEventBus(
        IOptions<RabbitMqEventBusOptions> options,
        IConnectionPool connectionPool,
        IRabbitMqSerializer serializer,
        IServiceScopeFactory serviceScopeFactory,
        IRabbitMqMessageConsumerFactory messageConsumerFactory,
        IGuidGenerator guidGenerator,
        IClock clock,
        IEventHandlerInvoker eventHandlerInvoker,
        ILogger<RabbitMqDistributedEventBus> logger,
        EventBusBuilder eventBusOptions)
        : base(serviceScopeFactory)
    {
        ConnectionPool = connectionPool;
        Serializer = serializer;
        GuidGenerator = guidGenerator;
        MessageConsumerFactory = messageConsumerFactory;
        RabbitMqEventBusOptions = options.Value;

        HandlerFactories = new ConcurrentDictionary<Type, List<IEventHandlerFactory>>();
        EventTypes = new ConcurrentDictionary<string, Type>();
        Logger = logger;
        EventHandlerInvoker = eventHandlerInvoker;
        EventBusOptions = eventBusOptions;
    }

    private async Task ProcessEventAsync(IModel channel, BasicDeliverEventArgs ea)
    {
        var eventName = ea.RoutingKey;
        var eventType = EventTypes.GetOrDefault(eventName);
        if (eventType == null)
        {
            return;
        }

        var eventData = Serializer.Deserialize(ea.Body.ToArray(), eventType);

        var correlationId = ea.BasicProperties.CorrelationId;
        Logger.LogDebug($"Event {eventName} received with correlation id: {correlationId}");


        await TriggerHandlersAsync(eventType, eventData);
    }

    public override IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
    {
        var handlerFactories = GetOrCreateHandlerFactories(eventType);

        if (factory.IsInFactories(handlerFactories))
        {
            return NullDisposable.Instance;
        }

        handlerFactories.Add(factory);

        if (handlerFactories.Count == 1) //TODO: Multi-threading!
        {
            Consumer.BindAsync(EventNameAttribute.GetNameOrDefault(eventType));
        }

        return new EventHandlerFactoryUnregistrar(this, eventType, factory);
    }

    /// <inheritdoc/>
    public override void Unsubscribe<TEvent>(Func<TEvent, Task> action)
    {
        action.NotNull(nameof(action));

        GetOrCreateHandlerFactories(typeof(TEvent))
            .Locking(factories =>
            {
                factories.RemoveAll(
                    factory =>
                    {
                        var singleInstanceFactory = factory as SingleInstanceHandlerFactory;
                        if (singleInstanceFactory == null)
                        {
                            return false;
                        }

                        var actionHandler = singleInstanceFactory.HandlerInstance as ActionEventHandler<TEvent>;
                        if (actionHandler == null)
                        {
                            return false;
                        }

                        return actionHandler.Action == action;
                    });
            });
    }

    /// <inheritdoc/>
    public override void Unsubscribe(Type eventType, IEventHandler handler)
    {
        GetOrCreateHandlerFactories(eventType)
            .Locking(factories =>
            {
                factories.RemoveAll(
                    factory =>
                        factory is SingleInstanceHandlerFactory &&
                        (factory as SingleInstanceHandlerFactory)!.HandlerInstance == handler
                );
            });
    }

    /// <inheritdoc/>
    public override void Unsubscribe(Type eventType, IEventHandlerFactory factory)
    {
        GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Remove(factory));
    }

    /// <inheritdoc/>
    public override void UnsubscribeAll(Type eventType)
    {
        GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Clear());
    }

    public virtual Task PublishAsync(
        Type eventType,
        object eventData,
        Dictionary<string, object>? headersArguments = null,
        Guid? eventId = null,
        string? correlationId = null)
    {
        var eventName = EventNameAttribute.GetNameOrDefault(eventType);
        var body = Serializer.Serialize(eventData);

        return PublishAsync(eventName, body, headersArguments, eventId, correlationId);
    }

    protected virtual Task PublishAsync(
        string eventName,
        byte[] body,
        Dictionary<string, object>? headersArguments = null,
        Guid? eventId = null,
        string? correlationId = null)
    {
        using (var channel = ConnectionPool.Get(RabbitMqEventBusOptions.ConnectionName).CreateModel())
        {
            return PublishAsync(channel, eventName, body, headersArguments, eventId, correlationId);
        }
    }

    protected virtual Task PublishAsync(
        IModel channel,
        string eventName,
        byte[] body,
        Dictionary<string, object>? headersArguments = null,
        Guid? eventId = null,
        string? correlationId = null)
    {
        EnsureExchangeExists(channel);

        var properties = channel.CreateBasicProperties();
        properties.DeliveryMode = RabbitMqConsts.DeliveryModes.Persistent;

        if (properties.MessageId.IsNullOrEmpty())
        {
            properties.MessageId = (eventId ?? GuidGenerator.Create()).ToString("N");
        }

        if (correlationId != null)
        {
            properties.CorrelationId = correlationId;
        }

        SetEventMessageHeaders(properties, headersArguments);

        channel.BasicPublish(
            exchange: RabbitMqEventBusOptions.ExchangeName,
            routingKey: eventName,
            mandatory: true,
            basicProperties: properties,
            body: body
        );

        return Task.CompletedTask;
    }

    private void EnsureExchangeExists(IModel channel)
    {
        if (_exchangeCreated)
        {
            return;
        }

        try
        {
            using (var temporaryChannel = ConnectionPool.Get(RabbitMqEventBusOptions.ConnectionName).CreateModel())
            {
                temporaryChannel.ExchangeDeclarePassive(RabbitMqEventBusOptions.ExchangeName);
            }
        }
        catch (Exception)
        {
            channel.ExchangeDeclare(
                RabbitMqEventBusOptions.ExchangeName,
                RabbitMqEventBusOptions.GetExchangeTypeOrDefault(),
                durable: true
            );
        }
        _exchangeCreated = true;
    }

    private void SetEventMessageHeaders(IBasicProperties properties, Dictionary<string, object>? headersArguments)
    {
        if (headersArguments == null)
        {
            return;
        }

        properties.Headers ??= new Dictionary<string, object>();

        foreach (var header in headersArguments)
        {
            properties.Headers[header.Key] = header.Value;
        }
    }

    private List<IEventHandlerFactory> GetOrCreateHandlerFactories(Type eventType)
    {
        return HandlerFactories.GetOrAdd(
            eventType,
            type =>
            {
                var eventName = EventNameAttribute.GetNameOrDefault(type);
                EventTypes.GetOrAdd(eventName, eventType);
                return new List<IEventHandlerFactory>();
            }
        );
    }

    protected override IEnumerable<EventTypeWithEventHandlerFactories> GetHandlerFactories(Type eventType)
    {
        var handlerFactoryList = new List<EventTypeWithEventHandlerFactories>();

        foreach (var handlerFactory in
                 HandlerFactories.Where(hf => ShouldTriggerEventForHandler(eventType, hf.Key)))
        {
            handlerFactoryList.Add(
                new EventTypeWithEventHandlerFactories(handlerFactory.Key, handlerFactory.Value));
        }

        return handlerFactoryList.ToArray();
    }

    private static bool ShouldTriggerEventForHandler(Type targetEventType, Type handlerEventType)
    {
        //Should trigger same type
        if (handlerEventType == targetEventType)
        {
            return true;
        }

        //TODO: Support inheritance? But it does not support on subscription to RabbitMq!
        //Should trigger for inherited types
        if (handlerEventType.IsAssignableFrom(targetEventType))
        {
            return true;
        }

        return false;
    }

    public override Task PublishAsync(Type eventType, object eventArgs)
    {
        var eventName = EventNameAttribute.GetNameOrDefault(eventType);
        var body = Serializer.Serialize(eventArgs);
        return PublishAsync(eventName, body);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Factory.StartNew(() =>
        {
            Consumer = MessageConsumerFactory.Create(
                new ExchangeDeclareConfiguration(
                    RabbitMqEventBusOptions.ExchangeName,
                    type: RabbitMqEventBusOptions.GetExchangeTypeOrDefault(),
                    durable: true,
                    arguments: RabbitMqEventBusOptions.ExchangeArguments
                ),
                new QueueDeclareConfiguration(
                    RabbitMqEventBusOptions.ClientName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    prefetchCount: RabbitMqEventBusOptions.PrefetchCount,
                    arguments: RabbitMqEventBusOptions.QueueArguments
                ),
                RabbitMqEventBusOptions.ConnectionName
            );

            Consumer.OnMessageReceived(ProcessEventAsync);
            SubscribeHandlers(EventBusOptions.Handlers);
            stoppingToken.WaitHandle.WaitOne(Timeout.Infinite);
            UnsubscribeHandlers(EventBusOptions.Handlers);
        }, stoppingToken);
    }

    public override void Dispose()
    {
        base.Dispose();
        Consumer?.Dispose();
        MessageConsumerFactory?.Dispose();
        GC.SuppressFinalize(this);
    }
}
