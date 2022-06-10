using Skywalker.PermissionsEvaluator.Abstractions;

namespace Skywalker.PermissionsEvaluator.AuthorizeValidation;

internal class RemotePermissionValidator : IPermissionValidator
{
    public Task<bool> IsGrantedAsync(string name, string providerName, string providerKey) => throw new NotImplementedException();
    
    public Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names, string providerName, string providerKey) => throw new NotImplementedException();
}
