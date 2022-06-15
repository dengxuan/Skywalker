using Microsoft.AspNetCore.Authorization;

namespace Skywalker.AspNetCore.Permissions.Abstractions;

public interface ISkywalkerAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    Task<List<string>> GetPoliciesNamesAsync();
}
