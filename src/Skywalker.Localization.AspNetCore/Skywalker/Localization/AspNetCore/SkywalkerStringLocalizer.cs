using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
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
    /// <remarks>
    /// Two placeholder styles are supported:
    /// <list type="bullet">
    /// <item><c>{0}</c> positional — <c>localizer["Hello {0}", name]</c>, plain <see cref="string.Format(string, object[])"/>.</item>
    /// <item><c>{name}</c> named — pass a single <see cref="IDictionary"/> / <c>IReadOnlyDictionary&lt;string, object?&gt;</c>
    /// or an anonymous object: <c>localizer["Hello {name}", new { name }]</c>.
    /// Missing named arguments are left as-is (<c>{name}</c>) rather than replaced with an empty string,
    /// so a caller that forgot an argument sees it on screen instead of silently losing the text.</item>
    /// </list>
    /// </remarks>
    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var localizedString = GetString(name);
            var value = arguments.Length == 1 && TryGetNamedArguments(arguments[0], out var named)
                ? Interpolate(localizedString.Value, named)
                : string.Format(localizedString.Value, arguments);
            return new LocalizedString(name, value, localizedString.ResourceNotFound, localizedString.SearchedLocation);
        }
    }

    private static readonly Regex NamedPlaceholder = new(@"\{([A-Za-z_][A-Za-z0-9_]*)\}", RegexOptions.Compiled);

    private static bool TryGetNamedArguments(object? candidate, out IReadOnlyDictionary<string, object?> named)
    {
        switch (candidate)
        {
            case IReadOnlyDictionary<string, object?> ro:
                named = ro;
                return true;
            case IDictionary<string, object?> d:
                named = new Dictionary<string, object?>(d);
                return true;
            case IDictionary legacy:
                var copy = new Dictionary<string, object?>();
                foreach (DictionaryEntry e in legacy)
                    if (e.Key is string k) copy[k] = e.Value;
                named = copy;
                return true;
            case null:
            case string:
            case IFormattable:   // numbers, dates, enums… → positional
                break;
            default:
                // anonymous / POCO: public readable properties become named arguments
                var props = candidate.GetType().GetProperties()
                    .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
                    .ToArray();
                if (props.Length > 0)
                {
                    named = props.ToDictionary(p => p.Name, p => p.GetValue(candidate));
                    return true;
                }
                break;
        }
        named = null!;
        return false;
    }

    private static string Interpolate(string template, IReadOnlyDictionary<string, object?> args)
        => NamedPlaceholder.Replace(template, m =>
            args.TryGetValue(m.Groups[1].Value, out var v) ? Convert.ToString(v, CultureInfo.CurrentCulture) ?? string.Empty : m.Value);

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
        var culturesToCheck = GetCulturesToCheck(CultureInfo.CurrentUICulture, includeParentCultures: true).ToList();
        foreach (var cultureName in culturesToCheck)
        {
            var localizedString = GetFromContributors(cultureName, name);
            if (localizedString != null)
            {
                return localizedString;
            }
        }

        // Whole current-culture chain missed → fall back to the default culture (resource-level first, then global)
        // instead of leaking the key onto the screen. ResourceNotFound stays true so callers can still tell it's a fallback.
        var fallbackCulture = _resource.DefaultCultureName ?? _options.DefaultCultureName;
        if (!string.IsNullOrEmpty(fallbackCulture)
            && !culturesToCheck.Contains(fallbackCulture, StringComparer.OrdinalIgnoreCase))
        {
            var fallback = GetFromContributors(fallbackCulture, name);
            if (fallback != null)
            {
                return new LocalizedString(name, fallback.Value, resourceNotFound: true, _resource.ResourceName);
            }
        }

        // Not found anywhere, return the name as value
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

