namespace Skywalker.Data.Filtering;

/// <summary>
/// Options for data filtering.
/// </summary>
public class DataFilterOptions
{
    /// <summary>
    /// Gets the default filter states.
    /// </summary>
    public Dictionary<Type, DataFilterState> DefaultStates { get; } = new();

    /// <summary>
    /// Configures the default state for a filter.
    /// </summary>
    /// <typeparam name="TFilter">The filter type.</typeparam>
    /// <param name="isEnabled">Whether the filter is enabled by default.</param>
    public void Configure<TFilter>(bool isEnabled) where TFilter : class
    {
        DefaultStates[typeof(TFilter)] = new DataFilterState(isEnabled);
    }
}

