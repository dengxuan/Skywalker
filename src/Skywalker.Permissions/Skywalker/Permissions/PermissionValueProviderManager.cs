// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skywalker.DependencyInjection;
using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions;

public class PermissionValueProviderManager : IPermissionValueProviderManager, ISingletonDependency
{
    public IReadOnlyList<IPermissionValueProvider> ValueProviders => _lazyProviders.Value;
    private readonly Lazy<List<IPermissionValueProvider>> _lazyProviders;

    protected PermissionOptions Options { get; }

    public PermissionValueProviderManager(
        IServiceProvider serviceProvider,
        IOptions<PermissionOptions> options)
    {
        Options = options.Value;

        _lazyProviders = new Lazy<List<IPermissionValueProvider>>(
            () => Options
                .ValueProviders
                .Select(c => (IPermissionValueProvider)serviceProvider.GetRequiredService(c))
                .ToList(),
            true
        );
    }
}
