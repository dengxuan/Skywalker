using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Skywalker.Messaging.Abstractions;
using StackExchange.Redis;

namespace Skywalker.Messaging.Redis
{
    /// <summary>
    /// A Redis Raw Message Publisher
    /// </summary>
    /// <inheritdoc />
    public class RedisMessagePublisher : IMessagePublisher
    {
        private readonly ISubscriber _subscriber;

        /// <summary>
        /// Creates an instance of <see cref="RedisMessagePublisher"/>
        /// </summary>
        /// <param name="connectionMultiplexer">Redis ConnectionMultiplexer</param>
        public RedisMessagePublisher(IConnectionMultiplexer connectionMultiplexer)
        {
            ISubscriber subscriber = connectionMultiplexer.NotNull(nameof(connectionMultiplexer)).GetSubscriber();
            _subscriber = subscriber ?? throw new ArgumentException("Redis Multiplexer returned no subscription.", nameof(connectionMultiplexer));
        }

        /// <summary>
        /// Publishes the raw message to the topic using Redis Pub/Sub
        /// </summary>
        /// <param name="routingKey">The topic to send the message to.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="_">Ignored token as SE.Redis doesn't support it.</param>
        /// <inheritdoc />
        public Task PublishAsync(string routingKey, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken)
        {
            return _subscriber.PublishAsync(routingKey, bytes);
        }
    }
}