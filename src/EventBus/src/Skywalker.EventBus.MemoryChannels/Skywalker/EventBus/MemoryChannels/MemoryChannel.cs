using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Skywalker.EventBus.MemoryChannels
{
    internal class MemoryChannel : IChannelModel, IConnection
    {
        private readonly CancellationTokenSource _cts = new();


        private readonly Channel<Message> _channel;


        private event Func<string, Task>? Closed;


        private event Func<string, ReadOnlyMemory<byte>, Task>? Received;

        Channel<Message> IChannelModel.Channel => _channel;


        public string RoutingKey { get; }


        public bool IsConnected { get; private set; }


        event Func<string, ReadOnlyMemory<byte>, Task>? IChannelModel.OnReceived
        {
            add
            {
                Received += value;
            }

            remove
            {
                Received -= value;
            }
        }


        event Func<string, Task>? IConnection.OnClosed
        {
            add
            {
                Closed += value;
            }

            remove
            {
                Closed -= value;
            }
        }

        public MemoryChannel(string routingKey, Channel<Message> channel)
        {
            RoutingKey = routingKey;
            _channel = channel;
        }

        public void Open()
        {
            Task.Run(async () =>
            {
                IsConnected = true;
                while (await _channel.Reader.WaitToReadAsync(_cts.Token))
                {
                    var message = await _channel.Reader.ReadAsync(_cts.Token);
                    Received?.Invoke(message.Id, message.Body)?.ConfigureAwait(false);
                }
                IsConnected = false;
                _channel.Writer.Complete();
            }, _cts.Token);
        }

        public async Task SendAsync(string routingKey, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default)
        {
            Message message = new(routingKey, bytes);
            await _channel.Writer.WriteAsync(message, cancellationToken);
        }

        public void Close()
        {
            Closed?.Invoke(RoutingKey)?.GetAwaiter();
            _cts.Cancel();
        }
    }
}
