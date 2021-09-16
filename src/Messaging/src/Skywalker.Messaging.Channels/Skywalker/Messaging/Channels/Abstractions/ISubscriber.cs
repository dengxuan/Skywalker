using System;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Messaging.Channels.Abstractions
{
    internal interface ISubscriber
    {
        Task PublishAsync(string routingKey, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken);

        Task SubscribeAsync(string routingKey, Func<string, ReadOnlyMemory<byte>, CancellationToken, Task> handler, CancellationToken cancellationToken);

        Task UnsubscribeAsync(string routingKey, CancellationToken cancellationToken);

        Task UnsubscribeAllAsync(CancellationToken cancellationToken);
    }
}
