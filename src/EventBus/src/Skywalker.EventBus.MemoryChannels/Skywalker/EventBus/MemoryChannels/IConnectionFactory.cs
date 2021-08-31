namespace Skywalker.EventBus.MemoryChannels;

public interface IConnectionFactory
{
    IConnection CreateChannel();
}
