using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Skywalker.EventBus.MemoryChannels
{
    public interface IChannelModel
    {

        string RoutingKey { get; }

        internal Channel<Message> Channel { get; }


        internal event Func<string, ReadOnlyMemory<byte>, Task>? OnReceived;
    }
}
