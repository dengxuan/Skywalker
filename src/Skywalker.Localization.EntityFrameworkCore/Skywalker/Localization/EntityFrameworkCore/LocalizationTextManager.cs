using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Skywalker.DependencyInjection;
using Skywalker.Localization.EntityFrameworkCore.Entities;

namespace Skywalker.Localization.EntityFrameworkCore;

/// <summary>
/// Default implementation of <see cref="ILocalizationTextManager"/>.
/// </summary>
public class LocalizationTextManager : ILocalizationTextManager, IScopedDependency
{
    private readonly ILocalizationDbContext _dbContext;
    private readonly IMemoryCache? _memoryCache;
    private const string CacheKeyPrefix = "Localization:";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Creates a new instance of <see cref="LocalizationTextManager"/>.
    /// </summary>
    public LocalizationTextManager(ILocalizationDbContext dbContext, IMemoryCache? memoryCache = null)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _memoryCache = memoryCache;
    }

    /// <inheritdoc/>
    public async Task<LocalizationText?> GetAsync(string resourceName, string cultureName, string key, CancellationToken cancellationToken = default)
    {
        return await _dbContext.LocalizationTexts
            .FirstOrDefaultAsync(x => x.ResourceName == resourceName && x.CultureName == cultureName && x.Key == key, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<LocalizationText>> GetListAsync(string resourceName, string cultureName, CancellationToken cancellationToken = default)
    {
        return await _dbContext.LocalizationTexts
            .Where(x => x.ResourceName == resourceName && x.CultureName == cultureName)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<LocalizationText>> GetListByResourceAsync(string resourceName, CancellationToken cancellationToken = default)
    {
        return await _dbContext.LocalizationTexts
            .Where(x => x.ResourceName == resourceName)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<LocalizationText> SetAsync(string resourceName, string cultureName, string key, string value, CancellationToken cancellationToken = default)
    {
        var existing = await GetAsync(resourceName, cultureName, key, cancellationToken);
        
        if (existing != null)
        {
            existing.SetValue(value);
            await _dbContext.SaveChangesAsync(true, cancellationToken);
            InvalidateCache(resourceName, cultureName);
            return existing;
        }

        var text = new LocalizationText(
            Guid.NewGuid().ToString("N"),
            resourceName,
            cultureName,
            key,
            value);

        await _dbContext.LocalizationTexts.AddAsync(text, cancellationToken);
        await _dbContext.SaveChangesAsync(true, cancellationToken);
        InvalidateCache(resourceName, cultureName);
        return text;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(string resourceName, string cultureName, string key, CancellationToken cancellationToken = default)
    {
        var existing = await GetAsync(resourceName, cultureName, key, cancellationToken);
        if (existing == null)
        {
            return false;
        }

        _dbContext.LocalizationTexts.Remove(existing);
        await _dbContext.SaveChangesAsync(true, cancellationToken);
        InvalidateCache(resourceName, cultureName);
        return true;
    }

    /// <inheritdoc/>
    public async Task<int> DeleteByCultureAsync(string resourceName, string cultureName, CancellationToken cancellationToken = default)
    {
        var texts = await GetListAsync(resourceName, cultureName, cancellationToken);
        if (texts.Count == 0)
        {
            return 0;
        }

        _dbContext.LocalizationTexts.RemoveRange(texts);
        await _dbContext.SaveChangesAsync(true, cancellationToken);
        InvalidateCache(resourceName, cultureName);
        return texts.Count;
    }

    /// <inheritdoc/>
    public async Task<int> ImportAsync(string resourceName, string cultureName, Dictionary<string, string> texts, bool overwrite = true, CancellationToken cancellationToken = default)
    {
        var existingTexts = await GetListAsync(resourceName, cultureName, cancellationToken);
        var existingDict = existingTexts.ToDictionary(x => x.Key, x => x, StringComparer.OrdinalIgnoreCase);
        var importedCount = 0;

        foreach (var (key, value) in texts)
        {
            if (existingDict.TryGetValue(key, out var existing))
            {
                if (overwrite && existing.Value != value)
                {
                    existing.SetValue(value);
                    importedCount++;
                }
            }
            else
            {
                var text = new LocalizationText(Guid.NewGuid().ToString("N"), resourceName, cultureName, key, value);
                await _dbContext.LocalizationTexts.AddAsync(text, cancellationToken);
                importedCount++;
            }
        }

        if (importedCount > 0)
        {
            await _dbContext.SaveChangesAsync(true, cancellationToken);
            InvalidateCache(resourceName, cultureName);
        }

        return importedCount;
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, string>> ExportAsync(string resourceName, string cultureName, CancellationToken cancellationToken = default)
    {
        var texts = await GetListAsync(resourceName, cultureName, cancellationToken);
        return texts.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetSupportedCulturesAsync(string resourceName, CancellationToken cancellationToken = default)
    {
        return await _dbContext.LocalizationTexts
            .Where(x => x.ResourceName == resourceName)
            .Select(x => x.CultureName)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public void InvalidateCache(string resourceName, string? cultureName = null)
    {
        if (_memoryCache == null) return;

        if (cultureName != null)
        {
            _memoryCache.Remove($"{CacheKeyPrefix}{resourceName}:{cultureName}");
        }
        _memoryCache.Remove($"{CacheKeyPrefix}{resourceName}:Cultures");
    }
}

