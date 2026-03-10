namespace Skywalker.Localization;

/// <summary>
/// Context for initializing a localization resource contributor.
/// </summary>
public class LocalizationResourceInitializationContext
{
    /// <summary>
    /// Gets the localization resource being initialized.
    /// </summary>
    public LocalizationResource Resource { get; }

    /// <summary>
    /// Gets the service provider for resolving dependencies.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Creates a new instance of <see cref="LocalizationResourceInitializationContext"/>.
    /// </summary>
    /// <param name="resource">The localization resource.</param>
    /// <param name="serviceProvider">The service provider.</param>
    public LocalizationResourceInitializationContext(LocalizationResource resource, IServiceProvider serviceProvider)
    {
        Resource = resource ?? throw new ArgumentNullException(nameof(resource));
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }
}

