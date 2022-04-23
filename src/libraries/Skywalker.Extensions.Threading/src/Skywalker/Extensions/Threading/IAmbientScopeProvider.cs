using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Extensions.Threading;

public interface IAmbientScopeProvider<T> : ISingletonDependency
{
    T? GetValue(string contextKey);

    IDisposable BeginScope(string contextKey, T value);
}
