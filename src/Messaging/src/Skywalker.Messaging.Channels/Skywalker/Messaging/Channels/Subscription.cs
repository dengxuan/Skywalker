using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Skywalker.Messaging.Channels
{
    internal sealed class Subscription : IDisposable
    {
        private bool disposedValue;

        private Task? _executeTask;

        private CancellationTokenSource? _stoppingCts;

        private readonly string _routingKey;

        private readonly Channel<ReadOnlyMemory<byte>> _channel;


        public event Func<string, ReadOnlyMemory<byte>, CancellationToken, Task>? OnReceived;

        public ChannelWriter<ReadOnlyMemory<byte>> Writer => _channel.Writer;

        public Subscription(string routingKay, Channel<ReadOnlyMemory<byte>> channel)
        {
            _routingKey = routingKay;
            _channel = channel;
        }

        private Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                while (await _channel.Reader.WaitToReadAsync(stoppingToken))
                {
                    var bytes = await _channel.Reader.ReadAsync(stoppingToken);
                    OnReceived?.Invoke(_routingKey, bytes, stoppingToken).ConfigureAwait(false);
                }
                _channel.Writer.TryComplete();
            }, stoppingToken);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _executeTask = ExecuteAsync(_stoppingCts.Token);
            if (_executeTask.IsCompleted)
            {
                return _executeTask;
            }
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executeTask == null)
            {
                return;
            }

            try
            {
                _channel.Writer.TryComplete();
                _stoppingCts?.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executeTask, Task.Delay(Timeout.Infinite, cancellationToken)).ConfigureAwait(false);
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _channel.Writer.TryComplete();
                    _stoppingCts?.Cancel();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        ///  Override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ///  Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        /// </summary>
        ~Subscription()
        {
            Dispose(disposing: false);
        }

        /// <summary>
        ///  Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
