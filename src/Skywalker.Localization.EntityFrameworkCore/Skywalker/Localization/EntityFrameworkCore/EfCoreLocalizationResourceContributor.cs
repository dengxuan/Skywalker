using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Localization.EntityFrameworkCore.Entities;

namespace Skywalker.Localization.EntityFrameworkCore;

/// <summary>
/// Contributes localized strings from EF Core database.
/// </summary>
public class EfCoreLocalizationResourceContributor : ILocalizationResourceContributor
{
    private readonly string _resourceName;
    private readonly TimeSpan _cacheExpiration;
    private IServiceProvider? _serviceProvider;
    private IMemoryCache? _memoryCache;

    // Cache key prefix
    private const string CacheKeyPrefix = "Localization:";

    /// <summary>
    /// Creates a new instance of <see cref="EfCoreLocalizationResourceContributor"/>.
    /// </summary>
    /// <param name="resourceName">The resource name to load from database.</param>
    /// <param name="cacheExpirationMinutes">Cache expiration in minutes (default: 30).</param>
    public EfCoreLocalizationResourceContributor(string resourceName, int cacheExpirationMinutes = 30)
    {
        Check.NotNullOrWhiteSpace(resourceName, nameof(resourceName));
        _resourceName = resourceName;
        _cacheExpiration = TimeSpan.FromMinutes(cacheExpirationMinutes);
    }

    /// <inheritdoc/>
    public void Initialize(LocalizationResourceInitializationContext context)
    {
        _serviceProvider = context.ServiceProvider;
        _memoryCache = _serviceProvider.GetService<IMemoryCache>();
    }

    /// <inheritdoc/>
    public LocalizedString? GetOrNull(string cultureName, string name)
    {
        var dictionary = GetCultureDictionary(cultureName);
        if (dictionary != null && dictionary.TryGetValue(name, out var value))
        {
            return new LocalizedString(name, value);
        }
        return null;
    }

    /// <inheritdoc/>
    public void Fill(string cultureName, Dictionary<string, LocalizedString> dictionary)
    {
        var source = GetCultureDictionary(cultureName);
        if (source != null)
        {
            foreach (var (key, value) in source)
            {
                dictionary[key] = new LocalizedString(key, value);
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetSupportedCultures()
    {
        if (_serviceProvider == null)
        {
            return Enumerable.Empty<string>();
        }

        var cacheKey = $"{CacheKeyPrefix}{_resourceName}:Cultures";

        if (_memoryCache != null && _memoryCache.TryGetValue(cacheKey, out List<string>? cultures) && cultures != null)
        {
            return cultures;
        }

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<ILocalizationDbContext>();
        if (dbContext == null)
        {
            return Enumerable.Empty<string>();
        }

        cultures = dbContext.LocalizationTexts
            .Where(x => x.ResourceName == _resourceName)
            .Select(x => x.CultureName)
            .Distinct()
            .ToList();

        _memoryCache?.Set(cacheKey, cultures, _cacheExpiration);
        return cultures;
    }

    private Dictionary<string, string>? GetCultureDictionary(string cultureName)
    {
        if (_serviceProvider == null)
        {
            return null;
        }

        var cacheKey = $"{CacheKeyPrefix}{_resourceName}:{cultureName}";

        if (_memoryCache != null && _memoryCache.TryGetValue(cacheKey, out Dictionary<string, string>? cached) && cached != null)
        {
            return cached;
        }

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<ILocalizationDbContext>();
        if (dbContext == null)
        {
            return null;
        }

        var dictionary = dbContext.LocalizationTexts
            .Where(x => x.ResourceName == _resourceName && x.CultureName == cultureName)
            .ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);

        _memoryCache?.Set(cacheKey, dictionary, _cacheExpiration);
        return dictionary;
    }

    /// <summary>
    /// Invalidates the cache for a specific culture or all cultures.
    /// </summary>
    /// <param name="cultureName">The culture name to invalidate, or null to invalidate all.</param>
    public void InvalidateCache(string? cultureName = null)
    {
        if (_memoryCache == null)
        {
            return;
        }

        if (cultureName != null)
        {
            var cacheKey = $"{CacheKeyPrefix}{_resourceName}:{cultureName}";
            _memoryCache.Remove(cacheKey);
        }

        // Always invalidate the cultures list
        _memoryCache.Remove($"{CacheKeyPrefix}{_resourceName}:Cultures");
    }
}

