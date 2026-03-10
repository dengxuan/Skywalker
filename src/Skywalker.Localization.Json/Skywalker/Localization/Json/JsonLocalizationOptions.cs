namespace Skywalker.Localization.Json;

/// <summary>
/// Options for JSON localization.
/// </summary>
public class JsonLocalizationOptions
{
    /// <summary>
    /// Gets or sets the base path for JSON resource files.
    /// </summary>
    public string? ResourcesPath { get; set; }

    /// <summary>
    /// Gets or sets whether to use the virtual file system.
    /// </summary>
    public bool UseVirtualFileSystem { get; set; } = true;

    /// <summary>
    /// Gets or sets the file extension for JSON resource files.
    /// Default is ".json".
    /// </summary>
    public string FileExtension { get; set; } = ".json";
}

