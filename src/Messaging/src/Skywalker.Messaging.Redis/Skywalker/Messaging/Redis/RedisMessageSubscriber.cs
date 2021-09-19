using Skywalker.Messaging.Abstractions;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Messaging.Redis
{
    /// <summary>
    /// A Redis Raw Message Handler Subscriber
    /// </summary>
    /// <inheritdoc />
    public class RedisRawMessageHandlerSubscriber : IMessageSubscriber
    {
        private readonly ISubscriber _subscriber;

        public RedisRawMessageHandlerSubscriber(IConnectionMultiplexer connectionMultiplexer)
        {
            ISubscriber subscriber = connectionMultiplexer.NotNull(nameof(connectionMultiplexer)).GetSubscriber();
            _subscriber = subscriber ?? throw new ArgumentException("Redis Multiplexer returned no subscription.", nameof(connectionMultiplexer));
        }

        /// <summary>
        /// Subscribes to the specified routingKey with Redis Pub/Sub
        /// </summary>
        /// <param name="routingKey"></param>
        /// <param name="rawHandler"></param>
        /// <param name="_"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public Task SubscribeAsync(string routingKey, Func<string, ReadOnlyMemory<byte>, CancellationToken, Task> rawHandler, CancellationToken cancellationToken)
        {
            void HandleRedisMessage(RedisChannel channel, RedisValue value)
            {
                rawHandler(channel, value, cancellationToken).ConfigureAwait(false);
            }

            return _subscriber.SubscribeAsync(routingKey, HandleRedisMessage);
        }

        public Task UnsubscribeAsync(string routingKey, CancellationToken cancellationToken = default)
        {
            return _subscriber.UnsubscribeAsync(routingKey);
        }
    }
}