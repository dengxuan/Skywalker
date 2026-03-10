namespace Skywalker.Localization;

/// <summary>
/// Represents a localization resource that contains localized strings.
/// </summary>
public class LocalizationResource : ILocalizationResource
{
    /// <summary>
    /// Gets the resource type used to identify this resource.
    /// </summary>
    public Type ResourceType { get; }

    /// <inheritdoc/>
    public string ResourceName { get; }

    /// <inheritdoc/>
    public string? DefaultCultureName { get; set; }

    /// <summary>
    /// Gets the base resource types that this resource inherits from.
    /// </summary>
    public List<Type> BaseResourceTypes { get; }

    /// <inheritdoc/>
    IReadOnlyList<Type> ILocalizationResource.BaseResourceTypes => BaseResourceTypes;

    /// <summary>
    /// Gets the contributors that provide localized strings.
    /// </summary>
    public List<ILocalizationResourceContributor> Contributors { get; }

    /// <inheritdoc/>
    IReadOnlyList<ILocalizationResourceContributor> ILocalizationResource.Contributors => Contributors;

    /// <summary>
    /// Creates a new instance of <see cref="LocalizationResource"/>.
    /// </summary>
    /// <param name="resourceType">The type that identifies this resource.</param>
    /// <param name="defaultCultureName">The default culture name.</param>
    public LocalizationResource(Type resourceType, string? defaultCultureName = null)
    {
        ResourceType = resourceType ?? throw new ArgumentNullException(nameof(resourceType));
        ResourceName = resourceType.FullName ?? resourceType.Name;
        DefaultCultureName = defaultCultureName;
        BaseResourceTypes = new List<Type>();
        Contributors = new List<ILocalizationResourceContributor>();
    }

    /// <summary>
    /// Adds a base resource type to inherit from.
    /// </summary>
    /// <typeparam name="TBaseResource">The base resource type.</typeparam>
    /// <returns>This resource for chaining.</returns>
    public LocalizationResource AddBaseTypes<TBaseResource>()
    {
        return AddBaseTypes(typeof(TBaseResource));
    }

    /// <summary>
    /// Adds base resource types to inherit from.
    /// </summary>
    /// <param name="types">The base resource types.</param>
    /// <returns>This resource for chaining.</returns>
    public LocalizationResource AddBaseTypes(params Type[] types)
    {
        foreach (var type in types)
        {
            if (!BaseResourceTypes.Contains(type))
            {
                BaseResourceTypes.Add(type);
            }
        }
        return this;
    }
}

