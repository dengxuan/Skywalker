using System;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Messaging.Abstractions;

/// <summary>
/// Message publisher
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Publishes the byte array into the specified routing key <paramref name="routingKey"/>
    /// </summary>
    /// <param name="routingKey">The destination to send the message</param>
    /// <param name="bytes">The deserialized message</param>
    /// <param name="cancellationToken">A cancellation token</param>
    Task PublishAsync(string routingKey, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default);
}
