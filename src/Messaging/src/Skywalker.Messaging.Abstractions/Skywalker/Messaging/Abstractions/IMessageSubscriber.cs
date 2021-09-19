using System;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Messaging.Abstractions;

/// <summary>
/// Subscribes raw message handlers to topics
/// </summary>
public interface IMessageSubscriber
{
    /// <summary>
    /// Subscribes to the specified routing key with the Handler <paramref name="handler"/>
    /// </summary>
    /// <param name="routingKey">The routingKey to subscribe to</param>
    /// <param name="handler">The handler to invoke</param>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <returns></returns>
    Task SubscribeAsync(string routingKey, Func<string, ReadOnlyMemory<byte>, CancellationToken, Task> handler, CancellationToken cancellationToken);

    /// <summary>
    /// Ubsubscribes to the specified routing key.
    /// </summary>
    /// <param name="routingKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UnsubscribeAsync(string routingKey, CancellationToken cancellationToken = default);
}
