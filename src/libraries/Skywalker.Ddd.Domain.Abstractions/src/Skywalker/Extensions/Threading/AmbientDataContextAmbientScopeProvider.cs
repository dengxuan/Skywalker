using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Skywalker.Extensions.Threading;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class AmbientDataContextAmbientScopeProvider<T> : IAmbientScopeProvider<T>
{

    private static readonly ConcurrentDictionary<string, ScopeItem> s_scopeDictionary = new();

    private readonly ILogger<AmbientDataContextAmbientScopeProvider<T>> _logger;

    private readonly IAmbientDataContext _dataContext;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataContext"></param>
    public AmbientDataContextAmbientScopeProvider(IAmbientDataContext dataContext, ILogger<AmbientDataContextAmbientScopeProvider<T>> logger)
    {
        _dataContext = dataContext;

        _logger = logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="contextKey"></param>
    /// <returns></returns>
    public T? GetValue(string contextKey)
    {
        var item = GetCurrentItem(contextKey);
        if (item == null)
        {
            return default;
        }

        return item.Value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="contextKey"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public IDisposable BeginScope(string contextKey, T value)
    {
        var item = new ScopeItem(value, GetCurrentItem(contextKey));

        if (!s_scopeDictionary.TryAdd(item.Id, item))
        {
            throw new Exception("Can not add item! ScopeDictionary.TryAdd returns false!");
        }
        _logger.LogDebug("BeginScope add item: {Id}", item.Id);
        _dataContext.SetData(contextKey, item.Id);

        return new DisposeAction(() =>
        {
            s_scopeDictionary.TryRemove(item.Id, out item);

            if (item?.Outer == null)
            {
                _dataContext.SetData(contextKey, null);
                return;
            }

            _dataContext.SetData(contextKey, item.Outer.Id);
        });
    }

    private ScopeItem? GetCurrentItem(string contextKey)
    {
        return _dataContext.GetData(contextKey) is string objKey ? s_scopeDictionary.GetOrDefault(objKey) : null;
    }

    private class ScopeItem
    {
        public string Id { get; }

        public ScopeItem? Outer { get; }

        public T Value { get; }

        public ScopeItem(T value, ScopeItem? outer = null)
        {
            Id = Guid.NewGuid().ToString();

            Value = value;
            Outer = outer;
        }
    }
}
