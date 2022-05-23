using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Authorization.Permissions.Abstractions;

public abstract class PermissionValueProvider : IPermissionValueProvider
{
    public abstract string Name { get; }

    protected IPermissionStore PermissionStore { get; }

    protected PermissionValueProvider(IPermissionStore permissionStore)
    {
        PermissionStore = permissionStore;
    }

    public abstract Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context);

    public abstract Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context);
}
