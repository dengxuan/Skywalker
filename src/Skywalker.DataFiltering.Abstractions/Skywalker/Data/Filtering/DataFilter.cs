using Microsoft.Extensions.Options;

namespace Skywalker.Data.Filtering;

/// <summary>
/// Default implementation of <see cref="IDataFilter"/>.
/// </summary>
public class DataFilter : IDataFilter
{
    private readonly DataFilterOptions _options;
    private readonly AsyncLocal<Dictionary<Type, DataFilterState>> _filters = new();

    /// <summary>
    /// Creates a new data filter.
    /// </summary>
    /// <param name="options">The filter options.</param>
    public DataFilter(IOptions<DataFilterOptions> options)
    {
        _options = options.Value;
    }

    /// <inheritdoc/>
    public bool IsEnabled<TFilter>() where TFilter : class
    {
        return GetFilterState<TFilter>().IsEnabled;
    }

    /// <inheritdoc/>
    public IDisposable Enable<TFilter>() where TFilter : class
    {
        return SetFilterState<TFilter>(true);
    }

    /// <inheritdoc/>
    public IDisposable Disable<TFilter>() where TFilter : class
    {
        return SetFilterState<TFilter>(false);
    }

    private DataFilterState GetFilterState<TFilter>() where TFilter : class
    {
        var filters = _filters.Value ??= new Dictionary<Type, DataFilterState>();
        var filterType = typeof(TFilter);

        if (filters.TryGetValue(filterType, out var state))
        {
            return state;
        }

        if (_options.DefaultStates.TryGetValue(filterType, out var defaultState))
        {
            return defaultState;
        }

        // Default to enabled
        return new DataFilterState(true);
    }

    private IDisposable SetFilterState<TFilter>(bool isEnabled) where TFilter : class
    {
        var filters = _filters.Value ??= new Dictionary<Type, DataFilterState>();
        var filterType = typeof(TFilter);

        var previousState = GetFilterState<TFilter>().Clone();
        filters[filterType] = new DataFilterState(isEnabled);

        return new DisposeAction(() =>
        {
            filters[filterType] = previousState;
        });
    }

    private sealed class DisposeAction : IDisposable
    {
        private readonly Action _action;
        private bool _disposed;

        public DisposeAction(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _action();
        }
    }
}

