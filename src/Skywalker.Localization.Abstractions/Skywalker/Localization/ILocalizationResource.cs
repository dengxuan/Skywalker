namespace Skywalker.Localization;

/// <summary>
/// Represents a localization resource that contains localized strings.
/// </summary>
public interface ILocalizationResource
{
    /// <summary>
    /// Gets the unique name of the resource.
    /// </summary>
    string ResourceName { get; }

    /// <summary>
    /// Gets the default culture name for this resource.
    /// </summary>
    string? DefaultCultureName { get; }

    /// <summary>
    /// Gets the base resources that this resource inherits from.
    /// </summary>
    IReadOnlyList<Type> BaseResourceTypes { get; }

    /// <summary>
    /// Gets the contributors that provide localized strings for this resource.
    /// </summary>
    IReadOnlyList<ILocalizationResourceContributor> Contributors { get; }
}

