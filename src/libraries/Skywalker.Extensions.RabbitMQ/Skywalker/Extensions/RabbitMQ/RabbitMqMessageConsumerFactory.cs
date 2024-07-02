using Microsoft.Extensions.DependencyInjection;
using Skywalker.Extensions.RabbitMQ.Abstractions;

namespace Skywalker.Extensions.RabbitMQ;

public class RabbitMqMessageConsumerFactory(IServiceScopeFactory serviceScopeFactory) : IRabbitMqMessageConsumerFactory, IDisposable
{
    protected IServiceScope ServiceScope { get; } = serviceScopeFactory.CreateScope();

    public IRabbitMqMessageConsumer Create(ExchangeDeclareConfiguration exchange, QueueDeclareConfiguration queue, string? connectionName = null)
    {
        var consumer = ServiceScope.ServiceProvider.GetRequiredService<IRabbitMqMessageConsumer>();
        consumer.Initialize(exchange, queue, connectionName);
        return consumer;
    }

    public void Dispose()
    {
        ServiceScope?.Dispose();
        GC.SuppressFinalize(this);
    }
}
