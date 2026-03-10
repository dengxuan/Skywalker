using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Skywalker.Localization.AspNetCore;

/// <summary>
/// Implementation of <see cref="IStringLocalizerFactory"/>.
/// </summary>
public class SkywalkerStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly LocalizationOptions _options;
    private readonly ConcurrentDictionary<Type, IStringLocalizer> _localizerCache;

    /// <summary>
    /// Creates a new instance of <see cref="SkywalkerStringLocalizerFactory"/>.
    /// </summary>
    public SkywalkerStringLocalizerFactory(
        IServiceProvider serviceProvider,
        IOptions<LocalizationOptions> options)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _localizerCache = new ConcurrentDictionary<Type, IStringLocalizer>();
    }

    /// <inheritdoc/>
    public IStringLocalizer Create<TResource>()
    {
        return Create(typeof(TResource));
    }

    /// <inheritdoc/>
    public IStringLocalizer Create(Type resourceType)
    {
        return _localizerCache.GetOrAdd(resourceType, type =>
        {
            var resource = _options.Resources.GetOrNull(type);

            if (resource == null)
            {
                // Return null localizer if resource not found
                return NullStringLocalizer.Instance;
            }

            // Initialize contributors
            var context = new LocalizationResourceInitializationContext(resource, _serviceProvider);
            foreach (var contributor in resource.Contributors)
            {
                contributor.Initialize(context);
            }

            return new SkywalkerStringLocalizer(
                resource,
                _serviceProvider.GetRequiredService<IOptions<LocalizationOptions>>());
        });
    }

    /// <inheritdoc/>
    public IStringLocalizer Create(string baseName, string location)
    {
        // Try to find by base name
        foreach (var resource in _options.Resources.Values)
        {
            if (resource.ResourceName.Equals(baseName, StringComparison.OrdinalIgnoreCase) ||
                resource.ResourceType.FullName?.Equals(baseName, StringComparison.OrdinalIgnoreCase) == true)
            {
                return Create(resource.ResourceType);
            }
        }

        // Fallback to default resource if configured
        if (_options.DefaultResourceType != null)
        {
            return Create(_options.DefaultResourceType);
        }

        return NullStringLocalizer.Instance;
    }
}

