using System.Globalization;

namespace Skywalker.Localization;

/// <summary>
/// A null implementation of <see cref="IStringLocalizer"/> that returns the key as the value.
/// </summary>
public class NullStringLocalizer : IStringLocalizer
{
    /// <summary>
    /// Gets the singleton instance of <see cref="NullStringLocalizer"/>.
    /// </summary>
    public static NullStringLocalizer Instance { get; } = new();

    private NullStringLocalizer()
    {
    }

    /// <inheritdoc/>
    public LocalizedString this[string name] => new(name, name, resourceNotFound: true);

    /// <inheritdoc/>
    public LocalizedString this[string name, params object[] arguments]
        => new(name, string.Format(name, arguments), resourceNotFound: true);

    /// <inheritdoc/>
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures = true)
        => Enumerable.Empty<LocalizedString>();

    /// <inheritdoc/>
    public IEnumerable<LocalizedString> GetAllStrings(CultureInfo culture, bool includeParentCultures = true)
        => Enumerable.Empty<LocalizedString>();
}

/// <summary>
/// A null implementation of <see cref="IStringLocalizer{TResource}"/> that returns the key as the value.
/// </summary>
/// <typeparam name="TResource">The resource type.</typeparam>
public class NullStringLocalizer<TResource> : IStringLocalizer<TResource>
{
    /// <summary>
    /// Gets the singleton instance of <see cref="NullStringLocalizer{TResource}"/>.
    /// </summary>
    public static NullStringLocalizer<TResource> Instance { get; } = new();

    private NullStringLocalizer()
    {
    }

    /// <inheritdoc/>
    public LocalizedString this[string name] => new(name, name, resourceNotFound: true);

    /// <inheritdoc/>
    public LocalizedString this[string name, params object[] arguments]
        => new(name, string.Format(name, arguments), resourceNotFound: true);

    /// <inheritdoc/>
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures = true)
        => Enumerable.Empty<LocalizedString>();

    /// <inheritdoc/>
    public IEnumerable<LocalizedString> GetAllStrings(CultureInfo culture, bool includeParentCultures = true)
        => Enumerable.Empty<LocalizedString>();
}

