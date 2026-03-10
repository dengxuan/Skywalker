using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Skywalker.Extensions.RabbitMQ.Abstractions;

public interface IRabbitMqMessageConsumer: IDisposable
{
    internal void Initialize(ExchangeDeclareConfiguration exchange, QueueDeclareConfiguration queue, string? connectionName = null);

    Task BindAsync(string routingKey);

    Task UnbindAsync(string routingKey);

    void OnMessageReceived(Func<IModel, BasicDeliverEventArgs, Task> callback);
}
