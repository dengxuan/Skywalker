using Microsoft.AspNetCore.Authorization;

namespace Skywalker.AspNetCore.Permissions;

public interface ISkywalkerAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    Task<List<string>> GetPoliciesNamesAsync();
}
