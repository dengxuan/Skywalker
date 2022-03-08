using Microsoft.AspNetCore.Authorization;

namespace Skywalker.AspNetCore.Authorization;

public interface ISkywalkerAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    Task<List<string>> GetPoliciesNamesAsync();
}
