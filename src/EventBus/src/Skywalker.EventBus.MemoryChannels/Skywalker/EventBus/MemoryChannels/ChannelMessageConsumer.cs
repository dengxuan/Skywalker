using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Skywalker.EventBus.MemoryChannels
{

    public class ChannelMessageConsumer : IChannelMessageConsumer
    {
        private readonly ConcurrentBag<Func<string, ReadOnlyMemory<byte>, Task>> _callbacks = new();

        private readonly ILogger<ChannelMessageConsumer> _logger;

        public ChannelMessageConsumer(ILogger<ChannelMessageConsumer> logger)
        {
            _logger = logger;
        }

        public void OnMessageReceived(Func<string, ReadOnlyMemory<byte>, Task> callback)
        {
            callback.NotNull(nameof(callback));
            _callbacks.Add(callback);
        }

        async Task IChannelMessageConsumer.InvokeAsync(string routingKey, ReadOnlyMemory<byte> bytes)
        {
            foreach (var callback in _callbacks)
            {
                try
                {
                    await callback(routingKey, bytes);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
    }
}
