using Skywalker.Messaging.Channels.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Skywalker.Messaging.Channels
{
    internal class Subscriber : ISubscriber
    {
        private readonly ConcurrentDictionary<string, Subscription> _subscriptions = new();

        public async Task PublishAsync(string routingKey, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default)
        {
            if (!_subscriptions.TryGetValue(routingKey, out Subscription? subscription))
            {
                throw new Exception();
            }
            await subscription.Writer.WriteAsync(bytes, cancellationToken);
        }

        public Task SubscribeAsync(string routingKey, Func<string, ReadOnlyMemory<byte>, CancellationToken, Task> handler, CancellationToken cancellationToken)
        {
            Subscription subscription = _subscriptions.GetOrAdd(routingKey, CreateSubscription);
            return Task.CompletedTask;
            Subscription CreateSubscription(string routingKey)
            {
                Subscription subscription = new(routingKey, Channel.CreateUnbounded<ReadOnlyMemory<byte>>());
                subscription.OnReceived += handler;
                subscription.StartAsync(cancellationToken);
                return subscription;
            }
        }

        public async Task UnsubscribeAsync(string routingKey, CancellationToken cancellationToken = default)
        {
            if (_subscriptions.TryRemove(routingKey, out var subscription))
            {
                await subscription.StopAsync(cancellationToken);
            }
        }

        public async Task UnsubscribeAllAsync(CancellationToken cancellationToken = default)
        {
            foreach (var (_, subscription) in _subscriptions)
            {
                await subscription.StopAsync(cancellationToken);
            }
            _subscriptions.Clear();
        }
    }
}
