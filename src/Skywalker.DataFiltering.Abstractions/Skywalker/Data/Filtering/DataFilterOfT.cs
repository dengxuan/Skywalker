namespace Skywalker.Data.Filtering;

/// <summary>
/// Typed data filter implementation.
/// </summary>
/// <typeparam name="TFilter">The filter type.</typeparam>
public class DataFilter<TFilter> : IDataFilter<TFilter> where TFilter : class
{
    private readonly IDataFilter _dataFilter;

    /// <summary>
    /// Creates a new typed data filter.
    /// </summary>
    /// <param name="dataFilter">The data filter.</param>
    public DataFilter(IDataFilter dataFilter)
    {
        _dataFilter = dataFilter;
    }

    /// <inheritdoc/>
    public bool IsEnabled => _dataFilter.IsEnabled<TFilter>();

    /// <inheritdoc/>
    public IDisposable Enable() => _dataFilter.Enable<TFilter>();

    /// <inheritdoc/>
    public IDisposable Disable() => _dataFilter.Disable<TFilter>();
}

