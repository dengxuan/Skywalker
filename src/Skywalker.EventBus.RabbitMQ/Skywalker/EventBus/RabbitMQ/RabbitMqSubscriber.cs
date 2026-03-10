// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.EventBus.Abstractions;
using Skywalker.Extensions.RabbitMQ;
using Skywalker.Extensions.RabbitMQ.Abstractions;

namespace Skywalker.EventBus.RabbitMQ;

internal class RabbitMqSubscriber(
    IRabbitMqSerializer rabbitMqSerializer,
    IEventHandlerInvoker eventHandlerInvoker,
    IOptions<EventBusOptions> busOptions,
    IOptions<RabbitMqEventBusOptions> options,
    IEventHandlerFactory eventHandlerFactory,
    IRabbitMqMessageConsumerFactory rabbitMqMessageConsumerFactory,
    ILogger<RabbitMqSubscriber> logger) : BackgroundService, ISubscriber
{
    private readonly EventBusOptions _busOptions = busOptions.Value;
    private readonly RabbitMqEventBusOptions _rabbitMqEventBusOptions = options.Value;

    private readonly List<IRabbitMqMessageConsumer> _consumers = [];

    public Dictionary<string, (Type eventType, Type handlerType)> GetEventHandlers()
    {
        var result = new Dictionary<string, (Type eventType, Type handlerType)>();

        foreach (var handlerType in _busOptions.Handlers)
        {
            var interfaces = handlerType.GetInterfaces();
            var handleType = interfaces.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>));
            if (handleType == null)
            {
                continue;
            }
            var genericArgs = handleType.GetGenericArguments();
            var eventType = genericArgs[0];
            var eventName = EventNameAttribute.GetNameOrDefault(eventType);
            result.Add(eventName, (eventType!, handlerType));
        }

        return result;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var handlers = GetEventHandlers();
            foreach (var (eventName, (eventType, handlerType)) in handlers)
            {
                var queueName = QueueNameAttribute.GetNameOrDefault(eventType) ?? eventName;

                var consumer = rabbitMqMessageConsumerFactory.Create(
                    new ExchangeDeclareConfiguration(
                        _rabbitMqEventBusOptions.ExchangeName,
                        type: _rabbitMqEventBusOptions.GetExchangeTypeOrDefault(),
                        durable: true,
                        arguments: _rabbitMqEventBusOptions.ExchangeArguments
                    ),
                    new QueueDeclareConfiguration(
                        $"{_rabbitMqEventBusOptions.ClientName}@{queueName}",
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        prefetchCount: _rabbitMqEventBusOptions.PrefetchCount,
                        arguments: _rabbitMqEventBusOptions.QueueArguments
                    ),
                    _rabbitMqEventBusOptions.ConnectionName
                );

                consumer.OnMessageReceived(async (m, b) =>
                {
                    var correlationId = b.BasicProperties.CorrelationId;
                    var routingKey = b.RoutingKey;

                    var (eventType, handlerType) = handlers.GetOrDefault(routingKey);
                    if (eventType == null || handlerType == null)
                    {
                        logger.LogWarning("Event {routingKey} received with correlation id: {correlationId} but no handler found", routingKey, correlationId);
                        return;
                    }
                    try
                    {
                        var handler = eventHandlerFactory.GetHandler(handlerType);
                        var eventData = rabbitMqSerializer.Deserialize(b.Body.ToArray(), eventType);
                        logger.LogDebug("Event {routingKey} received with correlation id: {correlationId}", routingKey, correlationId);
                        await eventHandlerInvoker.InvokeAsync(handler, eventData!, eventType);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Event {routingKey} received with correlation id: {correlationId} process error", routingKey, correlationId);
                        throw;
                    }
                });
                _consumers.Add(consumer);
                await consumer.BindAsync(eventName);
            }

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        finally
        {
            foreach (var consumer in _consumers)
            {
                consumer.Dispose();
            }
            _consumers.Clear();
        }
    }
}
