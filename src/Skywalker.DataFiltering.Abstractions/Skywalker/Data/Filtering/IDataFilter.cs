namespace Skywalker.Data.Filtering;

/// <summary>
/// Data filter interface for managing filter states.
/// </summary>
public interface IDataFilter
{
    /// <summary>
    /// Checks if the specified filter is enabled.
    /// </summary>
    /// <typeparam name="TFilter">The filter type.</typeparam>
    /// <returns>True if the filter is enabled.</returns>
    bool IsEnabled<TFilter>() where TFilter : class;

    /// <summary>
    /// Enables the specified filter.
    /// </summary>
    /// <typeparam name="TFilter">The filter type.</typeparam>
    /// <returns>A disposable that restores the previous state when disposed.</returns>
    IDisposable Enable<TFilter>() where TFilter : class;

    /// <summary>
    /// Disables the specified filter.
    /// </summary>
    /// <typeparam name="TFilter">The filter type.</typeparam>
    /// <returns>A disposable that restores the previous state when disposed.</returns>
    IDisposable Disable<TFilter>() where TFilter : class;
}

/// <summary>
/// Typed data filter interface.
/// </summary>
/// <typeparam name="TFilter">The filter type.</typeparam>
public interface IDataFilter<TFilter> where TFilter : class
{
    /// <summary>
    /// Gets whether the filter is enabled.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Enables the filter.
    /// </summary>
    /// <returns>A disposable that restores the previous state when disposed.</returns>
    IDisposable Enable();

    /// <summary>
    /// Disables the filter.
    /// </summary>
    /// <returns>A disposable that restores the previous state when disposed.</returns>
    IDisposable Disable();
}

