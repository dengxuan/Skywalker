using Skywalker.Messaging.Abstractions;
using Skywalker.Messaging.Channels.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Messaging.Channels
{
    internal class ChannelMessageSubscriber : IMessageSubscriber
    {
        private readonly ISubscriber _subscriber;

        public ChannelMessageSubscriber(ISubscriber subscriber)
        {
            _subscriber = subscriber;
        }

        public Task SubscribeAsync(string routingKey, Func<string, ReadOnlyMemory<byte>, CancellationToken, Task> handler, CancellationToken cancellationToken = default)
        {
            return _subscriber.SubscribeAsync(routingKey, handler, cancellationToken);
        }
    }
}
