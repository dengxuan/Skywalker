namespace Skywalker.Extensions.RabbitMQ.Abstractions;

public interface IChannelPool : IDisposable
{
    IChannelAccessor Acquire(string? channelName = null, string? connectionName = null);
}
