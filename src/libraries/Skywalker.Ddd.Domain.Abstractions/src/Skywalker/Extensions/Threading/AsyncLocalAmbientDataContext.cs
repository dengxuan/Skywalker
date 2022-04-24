using System.Collections.Concurrent;

namespace Skywalker.Extensions.Threading;

public class AsyncLocalAmbientDataContext : IAmbientDataContext
{
    private static readonly ConcurrentDictionary<string, AsyncLocal<object?>> AsyncLocalDictionary = new();

    public void SetData(string key, object? value)
    {
        var asyncLocal = AsyncLocalDictionary.GetOrAdd(key, (k) => new AsyncLocal<object?>());
        asyncLocal.Value = value;
    }

    public object? GetData(string key)
    {
        var asyncLocal = AsyncLocalDictionary.GetOrAdd(key, (k) => new AsyncLocal<object?>());
        return asyncLocal.Value;
    }
}
