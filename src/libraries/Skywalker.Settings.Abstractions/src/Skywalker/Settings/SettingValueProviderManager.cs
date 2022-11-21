using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skywalker.Settings.Abstractions;

namespace Skywalker.Settings;

public class SettingValueProviderManager : ISettingValueProviderManager//, ISingletonDependency
{
    public List<ISettingValueProvider> Providers => _lazyProviders.Value;
    protected SkywalkerSettingOptions Options { get; }
    private readonly Lazy<List<ISettingValueProvider>> _lazyProviders;

    public SettingValueProviderManager(
        IServiceProvider serviceProvider,
        IOptions<SkywalkerSettingOptions> options)
    {

        Options = options.Value;

        _lazyProviders = new Lazy<List<ISettingValueProvider>>(
            () => Options
                .ValueProviders
                .Select(type => serviceProvider.GetRequiredService(type) as ISettingValueProvider)
                .ToList(),
            true
        );
    }
}
