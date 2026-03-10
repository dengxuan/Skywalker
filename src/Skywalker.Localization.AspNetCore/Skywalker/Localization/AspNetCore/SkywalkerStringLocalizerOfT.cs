using System.Globalization;

namespace Skywalker.Localization.AspNetCore;

/// <summary>
/// Typed implementation of <see cref="IStringLocalizer{TResource}"/>.
/// </summary>
/// <typeparam name="TResource">The resource type.</typeparam>
public class SkywalkerStringLocalizer<TResource> : IStringLocalizer<TResource>
{
    private readonly IStringLocalizer _innerLocalizer;

    /// <summary>
    /// Creates a new instance of <see cref="SkywalkerStringLocalizer{TResource}"/>.
    /// </summary>
    public SkywalkerStringLocalizer(IStringLocalizerFactory factory)
    {
        _innerLocalizer = factory.Create<TResource>();
    }

    /// <inheritdoc/>
    public LocalizedString this[string name] => _innerLocalizer[name];

    /// <inheritdoc/>
    public LocalizedString this[string name, params object[] arguments] => _innerLocalizer[name, arguments];

    /// <inheritdoc/>
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures = true)
        => _innerLocalizer.GetAllStrings(includeParentCultures);

    /// <inheritdoc/>
    public IEnumerable<LocalizedString> GetAllStrings(CultureInfo culture, bool includeParentCultures = true)
        => _innerLocalizer.GetAllStrings(culture, includeParentCultures);
}

