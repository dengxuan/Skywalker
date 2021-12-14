using System.Threading;

namespace Skywalker.Extensions.Threading
{
    public interface ICancellationTokenProvider
    {
        CancellationToken Token { get; }
    }
}
