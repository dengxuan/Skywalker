using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions;

public class PermissionValueProviderManager : IPermissionValueProviderManager
{
    public IReadOnlyList<IPermissionValueProvider?> ValueProviders => _lazyProviders.Value;
    private readonly Lazy<List<IPermissionValueProvider?>> _lazyProviders;

    protected PermissionOptions Options { get; }

    public PermissionValueProviderManager(
        IServiceProvider serviceProvider,
        IOptions<PermissionOptions> options)
    {
        Options = options.Value;

        _lazyProviders = new Lazy<List<IPermissionValueProvider?>>(
            () => Options
                .ValueProviders
                .Select(c => serviceProvider.GetRequiredService(c) as IPermissionValueProvider)
                .ToList(),
            true
        );
    }
}
