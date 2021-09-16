using Skywalker.Messaging.Abstractions;
using Skywalker.Messaging.Channels.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Messaging.Channels
{
    internal class ChannelMessagePublisher : IMessagePublisher
    {
        private readonly ISubscriber _subscriber;

        public ChannelMessagePublisher(ISubscriber subscriber)
        {
            _subscriber = subscriber;
        }

        public Task PublishAsync(string routingKey, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken)
        {
            return _subscriber.PublishAsync(routingKey, bytes, cancellationToken);
        }
    }
}
