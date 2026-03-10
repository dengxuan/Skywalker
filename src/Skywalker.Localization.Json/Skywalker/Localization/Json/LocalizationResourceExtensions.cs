namespace Skywalker.Localization.Json;

/// <summary>
/// Extension methods for <see cref="LocalizationResource"/> to add JSON contributors.
/// </summary>
public static class LocalizationResourceExtensions
{
    /// <summary>
    /// Adds a JSON localization resource contributor.
    /// </summary>
    /// <param name="resource">The localization resource.</param>
    /// <param name="virtualPath">The virtual path to the JSON resource files.</param>
    /// <param name="fileExtension">The file extension (default: .json).</param>
    /// <returns>The localization resource for chaining.</returns>
    public static LocalizationResource AddJson(
        this LocalizationResource resource,
        string virtualPath,
        string fileExtension = ".json")
    {
        resource.Contributors.Add(new JsonLocalizationResourceContributor(virtualPath, fileExtension));
        return resource;
    }

    /// <summary>
    /// Adds a JSON localization resource contributor using the default path based on resource type.
    /// </summary>
    /// <param name="resource">The localization resource.</param>
    /// <param name="basePath">The base path for localization resources.</param>
    /// <returns>The localization resource for chaining.</returns>
    public static LocalizationResource AddJsonFromDefaultPath(
        this LocalizationResource resource,
        string basePath = "/Localization")
    {
        var resourceName = resource.ResourceType.Name;
        var virtualPath = $"{basePath}/{resourceName}";
        return resource.AddJson(virtualPath);
    }
}

