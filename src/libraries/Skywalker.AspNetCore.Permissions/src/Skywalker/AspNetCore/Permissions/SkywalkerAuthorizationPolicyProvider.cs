using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Skywalker.AspNetCore.Permissions.Abstractions;
using Skywalker.Permissions.Abstractions;
using System.Collections.Concurrent;

namespace Skywalker.AspNetCore.Permissions;

public class SkywalkerAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider, ISkywalkerAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _options;
    private readonly IPermissionDefinitionManager _permissionDefinitionManager;
    private readonly ConcurrentDictionary<string, AuthorizationPolicy> _policies = new();

    public SkywalkerAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options, IPermissionDefinitionManager permissionDefinitionManager) : base(options)
    {
        _permissionDefinitionManager = permissionDefinitionManager;
        _options = options.Value;
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = await base.GetPolicyAsync(policyName);
        if (policy != null)
        {
            return policy;
        }

        var permission = await _permissionDefinitionManager.GetOrNullAsync(policyName);
        if (permission == null)
        {
            //权限发生更改后,尝试移除已经缓存的策略
            _policies.TryRemove(policyName, out _);
            return null;
        }
        //对于已经授权的策略, 构建权限验证策略,并缓存
        return _policies.GetOrAdd(permission.Name, () =>
        {
            var policyBuilder = new AuthorizationPolicyBuilder(Array.Empty<string>());
            policyBuilder.Requirements.Add(new PermissionRequirement(policyName));
            return policyBuilder.Build();
        });
    }

    public async Task<List<string>> GetPoliciesNamesAsync()
    {
        var permissionDefinitions = await _permissionDefinitionManager.GetPermissionsAsync();
        var result = _options.GetPoliciesNames().Union(permissionDefinitions.Select(p => p.Name)).ToList();
        return result;
    }
}
