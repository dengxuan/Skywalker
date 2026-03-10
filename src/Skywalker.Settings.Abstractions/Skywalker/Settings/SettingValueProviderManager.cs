using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skywalker.Settings.Abstractions;

namespace Skywalker.Settings;

public class SettingValueProviderManager : ISettingValueProviderManager
{
    private readonly Lazy<List<ISettingValueProvider>> _lazyProviders;

    protected SkywalkerSettingOptions Options { get; }

    public SettingValueProviderManager(IServiceProvider serviceProvider, IOptions<SkywalkerSettingOptions> options)
    {
        Options = options.Value;

        _lazyProviders = new Lazy<List<ISettingValueProvider>>(() =>
        {
            var providers = new List<ISettingValueProvider>();

            // Create providers in the order defined in Options.ValueProviders
            foreach (var providerType in Options.ValueProviders)
            {
                var provider = (ISettingValueProvider)serviceProvider.GetRequiredService(providerType);
                providers.Add(provider);
            }

            return providers;
        }, true);
    }

    public List<ISettingValueProvider> Providers => _lazyProviders.Value;
}
