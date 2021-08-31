using System;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.EventBus.MemoryChannels
{
    public interface IConnection
    {
        bool IsConnected { get; }

        internal event Func<string, Task>? OnClosed;

        internal void Open();


        Task SendAsync(string routingKey, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default);

        void Close();
    }
}
