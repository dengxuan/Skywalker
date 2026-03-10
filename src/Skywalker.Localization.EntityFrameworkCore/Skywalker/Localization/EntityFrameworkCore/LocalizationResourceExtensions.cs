namespace Skywalker.Localization.EntityFrameworkCore;

/// <summary>
/// Extension methods for <see cref="LocalizationResource"/> to add EF Core contributors.
/// </summary>
public static class LocalizationResourceExtensions
{
    /// <summary>
    /// Adds an EF Core database localization resource contributor.
    /// </summary>
    /// <param name="resource">The localization resource.</param>
    /// <param name="resourceName">The resource name in the database (defaults to resource type full name).</param>
    /// <param name="cacheExpirationMinutes">Cache expiration in minutes (default: 30).</param>
    /// <returns>The localization resource for chaining.</returns>
    public static LocalizationResource AddEfCore(
        this LocalizationResource resource,
        string? resourceName = null,
        int cacheExpirationMinutes = 30)
    {
        resource.Contributors.Add(new EfCoreLocalizationResourceContributor(
            resourceName ?? resource.ResourceName,
            cacheExpirationMinutes));
        return resource;
    }
}

