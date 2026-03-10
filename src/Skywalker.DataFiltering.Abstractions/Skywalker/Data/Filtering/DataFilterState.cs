namespace Skywalker.Data.Filtering;

/// <summary>
/// Represents the state of a data filter.
/// </summary>
public class DataFilterState
{
    /// <summary>
    /// Gets or sets whether the filter is enabled.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Creates a new filter state.
    /// </summary>
    /// <param name="isEnabled">Whether the filter is enabled.</param>
    public DataFilterState(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }

    /// <summary>
    /// Creates a clone of this state.
    /// </summary>
    /// <returns>A new state with the same values.</returns>
    public DataFilterState Clone()
    {
        return new DataFilterState(IsEnabled);
    }
}

