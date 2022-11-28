using System.Collections.Concurrent;

namespace Skywalker.Extensions.Threading;

public class AsyncLocalAmbientDataContext : IAmbientDataContext
{
    private static readonly ConcurrentDictionary<string, AsyncLocal<object?>> s_asyncLocalDictionary = new();

    public void SetData(string key, object? value)
    {
        var asyncLocal = s_asyncLocalDictionary.GetOrAdd(key, (k) => new AsyncLocal<object?>());
        asyncLocal.Value = value;
    }

    public object? GetData(string key)
    {
        var asyncLocal = s_asyncLocalDictionary.GetOrAdd(key, (k) => new AsyncLocal<object?>());
        return asyncLocal.Value;
    }
}
