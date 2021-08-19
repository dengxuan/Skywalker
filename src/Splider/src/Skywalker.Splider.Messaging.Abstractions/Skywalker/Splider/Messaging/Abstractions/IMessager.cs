using System;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Splider.Messaging.Abstractions;

public interface IMessager : IDisposable
{
    Task PublishAsync(string queue, byte[] message);

    Task ConsumeAsync(MessageConsumer<byte[]> consumer, CancellationToken cancellationToken);

    void CloseQueue(string queue);

    bool IsDistributed { get; }
}
