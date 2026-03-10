using RabbitMQ.Client;

namespace Skywalker.Extensions.RabbitMQ.Abstractions;

public interface IConnectionPool : IDisposable
{
    IConnection Get(string? connectionName = null);
}
