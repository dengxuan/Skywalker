// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Skywalker.Permissions.AspNetCore;

/// <summary>
/// 动态权限策略提供者
/// 支持动态创建权限策略，无需预先注册所有权限
/// </summary>
public class PermissionAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

    public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return _fallbackPolicyProvider.GetDefaultPolicyAsync();
    }

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return _fallbackPolicyProvider.GetFallbackPolicyAsync();
    }

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // 如果策略名以 "Permission:" 开头，动态创建权限策略
        if (policyName.StartsWith(RequirePermissionAttribute.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var permission = policyName.Substring(RequirePermissionAttribute.PolicyPrefix.Length);
            
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(permission))
                .Build();
            
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        // 否则使用默认的策略提供者
        return _fallbackPolicyProvider.GetPolicyAsync(policyName);
    }
}
