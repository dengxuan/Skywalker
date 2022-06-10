using Microsoft.AspNetCore.Authorization;

namespace Skywalker.AspNetCore.PermissionsEvaluator;

public interface ISkywalkerAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    Task<List<string>> GetPoliciesNamesAsync();
}
