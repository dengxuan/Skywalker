namespace Skywalker.Template.Scriban;

/// <summary>
/// Options for Scriban template rendering.
/// </summary>
public class ScribanTemplateOptions
{
    /// <summary>
    /// Gets or sets whether to enable template caching.
    /// Default is true.
    /// </summary>
    public bool EnableCaching { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of cached templates.
    /// Default is 1000.
    /// </summary>
    public int MaxCacheSize { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the global functions to be registered.
    /// </summary>
    public Dictionary<string, Delegate> GlobalFunctions { get; } = new();

    /// <summary>
    /// Gets or sets the global variables to be registered.
    /// </summary>
    public Dictionary<string, object> GlobalVariables { get; } = new();
}

