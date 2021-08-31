using Skywalker.Spider.Messaging.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Skywalker.Spider.Messaging;

public class DefaultMessager : IMessager
{
    private readonly ConcurrentDictionary<string, Channel<byte[]>> _channels = new();

    public async Task PublishAsync(string queue, byte[] message)
    {
        var channel = _channels.GetOrAdd(queue, _ => Channel.CreateUnbounded<byte[]>());
        await channel.Writer.WriteAsync(message);
    }

    public async Task ConsumeAsync(MessageConsumer<byte[]> consumer, CancellationToken cancellationToken)
    {
        var channel = _channels.GetOrAdd(consumer.Queue, _ => Channel.CreateUnbounded<byte[]>());
        consumer.OnClosing += x => CloseQueue(x.Queue);

        await Task.Run(async () =>
        {
            while (await channel.Reader.WaitToReadAsync(cancellationToken))
            {
                var bytes = await channel.Reader.ReadAsync(cancellationToken);
                await Task.Factory.StartNew(async () =>
                {
                    await consumer.InvokeAsync(bytes);
                }, cancellationToken)
                .ConfigureAwait(false);
            }
        }, cancellationToken);
    }

    public void CloseQueue(string queue)
    {
        if (_channels.TryRemove(queue, out var channel))
        {
            try
            {
                channel.Writer.Complete();
            }
            catch
            {
                // ignore
            }
        }
    }

    public bool IsDistributed => false;

    public void Dispose()
    {
        foreach (var kv in _channels)
        {
            kv.Value.Writer.Complete();
        }

        _channels.Clear();
    }
}
