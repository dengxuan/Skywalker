using System;
using System.Threading.Tasks;

namespace Skywalker.EventBus.MemoryChannels;

public interface IChannelMessageConsumer
{

    void OnMessageReceived(Func<string, ReadOnlyMemory<byte>, Task> callback);

    internal Task InvokeAsync(string routingKey, ReadOnlyMemory<byte> bytes);
}
