namespace Skywalker.Localization;

/// <summary>
/// Factory for creating <see cref="IStringLocalizer"/> instances.
/// </summary>
public interface IStringLocalizerFactory
{
    /// <summary>
    /// Creates an <see cref="IStringLocalizer"/> for the specified resource type.
    /// </summary>
    /// <typeparam name="TResource">The resource type.</typeparam>
    /// <returns>The string localizer.</returns>
    IStringLocalizer Create<TResource>();

    /// <summary>
    /// Creates an <see cref="IStringLocalizer"/> for the specified resource type.
    /// </summary>
    /// <param name="resourceType">The resource type.</param>
    /// <returns>The string localizer.</returns>
    IStringLocalizer Create(Type resourceType);

    /// <summary>
    /// Creates an <see cref="IStringLocalizer"/> for the specified base name and location.
    /// </summary>
    /// <param name="baseName">The base name of the resource.</param>
    /// <param name="location">The location to search for resources.</param>
    /// <returns>The string localizer.</returns>
    IStringLocalizer Create(string baseName, string location);
}

