namespace Skywalker.Template.Razor;

/// <summary>
/// Options for Razor template rendering.
/// </summary>
public class RazorTemplateOptions
{
    /// <summary>
    /// Gets or sets whether to enable template caching.
    /// Default is true.
    /// </summary>
    public bool EnableCaching { get; set; } = true;

    /// <summary>
    /// Gets or sets the default namespace for compiled templates.
    /// </summary>
    public string DefaultNamespace { get; set; } = "SkywalkerTemplates";

    /// <summary>
    /// Gets or sets whether to enable debugging.
    /// Default is false.
    /// </summary>
    public bool EnableDebugging { get; set; } = false;
}

