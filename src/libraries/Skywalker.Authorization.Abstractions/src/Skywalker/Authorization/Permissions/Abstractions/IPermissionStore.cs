using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Authorization.Permissions.Abstractions;

public interface IPermissionStore : ISingletonDependency
{
    Task<bool> IsGrantedAsync(string name, string providerName, string providerKey);

    Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names, string providerName, string providerKey);
}
