using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.PermissionsEvaluator.Abstractions;

public interface IPermissionValidator : ISingletonDependency
{
    Task<bool> IsGrantedAsync(string name, string providerName, string providerKey);

    Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names, string providerName, string providerKey);
}
