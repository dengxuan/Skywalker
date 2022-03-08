using Skywalker.Authorization.Permissions;

namespace Skywalker.Authorization.Permissions.Abstractions;

public interface IPermissionStore
{
    Task<bool> IsGrantedAsync(string name, string? providerName, string? providerKey);

    Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names, string? providerName, string? providerKey);
}
