using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Extensions.Threading;

public interface ICancellationTokenProvider: ISingletonDependency
{
    CancellationToken Token { get; }
}
