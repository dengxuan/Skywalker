using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Skywalker.Extensions.GuidGenerator;
using Skywalker.Extensions.RabbitMQ;
using Skywalker.Extensions.RabbitMQ.Abstractions;

namespace Skywalker.EventBus.RabbitMQ;

public class RabbitMqEventBus(
    IOptions<RabbitMqEventBusOptions> options,
    IConnectionPool connectionPool,
    IRabbitMqSerializer serializer,
    IGuidGenerator guidGenerator,
    ILogger<RabbitMqEventBus> logger) : EventBusBase
{
    protected RabbitMqEventBusOptions RabbitMqEventBusOptions { get; } = options.Value;

    protected IConnectionPool ConnectionPool { get; } = connectionPool;

    protected IRabbitMqSerializer Serializer { get; } = serializer;

    protected IGuidGenerator GuidGenerator { get; } = guidGenerator;

    protected ILogger<RabbitMqEventBus> Logger { get; } = logger;

    private volatile bool _exchangeCreated;
    private readonly object _exchangeLock = new();

    private void EnsureExchangeExists(IModel channel)
    {
        if (_exchangeCreated)
        {
            return;
        }

        lock (_exchangeLock)
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
    }

    private static void SetEventMessageHeaders(IBasicProperties properties, Dictionary<string, object>? headersArguments)
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

    public virtual Task PublishAsync(Type eventType, object eventData, Dictionary<string, object>? headersArguments = null, Guid? eventId = null, string? correlationId = null)
    {
        var routingKey = EventNameAttribute.GetNameOrDefault(eventType);
        var body = Serializer.Serialize(eventData);

        return PublishAsync(routingKey, body, headersArguments, eventId, correlationId);
    }

    protected virtual Task PublishAsync(string routingKey, byte[] body, Dictionary<string, object>? headersArguments = null, Guid? eventId = null, string? correlationId = null)
    {
        using (var channel = ConnectionPool.Get(RabbitMqEventBusOptions.ConnectionName).CreateModel())
        {
            return PublishAsync(channel, routingKey, body, headersArguments, eventId, correlationId);
        }
    }

    protected virtual Task PublishAsync(IModel channel, string routingKey, byte[] body, Dictionary<string, object>? headersArguments = null, Guid? eventId = null, string? correlationId = null)
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
            routingKey: routingKey,
            mandatory: true,
            basicProperties: properties,
            body: body
        );
        Logger.LogDebug("Event {routingKey} published with correlation id: {correlationId}", routingKey, correlationId);
        return Task.CompletedTask;
    }

    public override Task PublishAsync(Type eventType, object eventArgs)
    {
        var routingKey = EventNameAttribute.GetNameOrDefault(eventType);
        var body = Serializer.Serialize(eventArgs);
        return PublishAsync(routingKey, body);
    }
}
