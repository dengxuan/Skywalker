using System.Globalization;
using Microsoft.Extensions.Options;

namespace Skywalker.Localization.AspNetCore;

/// <summary>
/// Implementation of <see cref="IStringLocalizer"/> that uses <see cref="LocalizationResource"/>.
/// </summary>
public class SkywalkerStringLocalizer : IStringLocalizer
{
    private readonly LocalizationResource _resource;
    private readonly LocalizationOptions _options;

    /// <summary>
    /// Creates a new instance of <see cref="SkywalkerStringLocalizer"/>.
    /// </summary>
    public SkywalkerStringLocalizer(LocalizationResource resource, IOptions<LocalizationOptions> options)
    {
        _resource = resource ?? throw new ArgumentNullException(nameof(resource));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc/>
    public LocalizedString this[string name] => GetString(name);

    /// <inheritdoc/>
    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var localizedString = GetString(name);
            return new LocalizedString(
                name,
                string.Format(localizedString.Value, arguments),
                localizedString.ResourceNotFound,
                localizedString.SearchedLocation);
        }
    }

    /// <inheritdoc/>
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures = true)
    {
        return GetAllStrings(CultureInfo.CurrentUICulture, includeParentCultures);
    }

    /// <inheritdoc/>
    public IEnumerable<LocalizedString> GetAllStrings(CultureInfo culture, bool includeParentCultures = true)
    {
        var result = new Dictionary<string, LocalizedString>();
        var culturesToCheck = GetCulturesToCheck(culture, includeParentCultures);

        foreach (var cultureName in culturesToCheck)
        {
            FillFromContributors(cultureName, result);
        }

        return result.Values;
    }

    private LocalizedString GetString(string name)
    {
        var culturesToCheck = GetCulturesToCheck(CultureInfo.CurrentUICulture, includeParentCultures: true);

        foreach (var cultureName in culturesToCheck)
        {
            var localizedString = GetFromContributors(cultureName, name);
            if (localizedString != null)
            {
                return localizedString;
            }
        }

        // Not found, return the name as value
        return new LocalizedString(name, name, resourceNotFound: true, _resource.ResourceName);
    }

    private LocalizedString? GetFromContributors(string cultureName, string name)
    {
        // Check resource contributors
        foreach (var contributor in _resource.Contributors)
        {
            var result = contributor.GetOrNull(cultureName, name);
            if (result != null)
            {
                return result;
            }
        }

        // Check global contributors
        foreach (var contributor in _options.GlobalContributors)
        {
            var result = contributor.GetOrNull(cultureName, name);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    private void FillFromContributors(string cultureName, Dictionary<string, LocalizedString> dictionary)
    {
        // Fill from global contributors first (can be overwritten by resource contributors)
        foreach (var contributor in _options.GlobalContributors)
        {
            contributor.Fill(cultureName, dictionary);
        }

        // Fill from resource contributors
        foreach (var contributor in _resource.Contributors)
        {
            contributor.Fill(cultureName, dictionary);
        }
    }

    private static IEnumerable<string> GetCulturesToCheck(CultureInfo culture, bool includeParentCultures)
    {
        var cultures = new List<string> { culture.Name };

        if (includeParentCultures)
        {
            var parent = culture.Parent;
            while (parent != CultureInfo.InvariantCulture)
            {
                cultures.Add(parent.Name);
                parent = parent.Parent;
            }
        }

        return cultures;
    }
}

