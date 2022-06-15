// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Security.Claims;
using Skywalker.Extensions.SimpleStateChecking;
using Skywalker.Permissions.Abstractions;
using Skywalker.Security.Claims;

namespace Skywalker.Permissions;

public class PermissionChecker : IPermissionChecker
{
    private readonly ICurrentPrincipalAccessor _principalAccessor;
    private readonly IPermissionDefinitionManager _permissionDefinitionManager;
    private readonly IPermissionValueProviderManager _permissionValueProviderManager;
    private readonly ISimpleStateCheckerManager<PermissionDefinition> _stateCheckerManager;

    public PermissionChecker(
        ICurrentPrincipalAccessor principalAccessor,
        IPermissionDefinitionManager permissionDefinitionManager,
        IPermissionValueProviderManager permissionValueProviderManager,
        ISimpleStateCheckerManager<PermissionDefinition> stateCheckerManager)
    {
        _principalAccessor = principalAccessor;
        _permissionDefinitionManager = permissionDefinitionManager;
        _permissionValueProviderManager = permissionValueProviderManager;
        _stateCheckerManager = stateCheckerManager;
    }

    public virtual async Task<bool> IsGrantedAsync(string name)
    {
        return await IsGrantedAsync(_principalAccessor.Principal, name);
    }

    public virtual async Task<bool> IsGrantedAsync(ClaimsPrincipal? claimsPrincipal, string name)
    {
        var permission = await _permissionDefinitionManager.GetAsync(name);
        
        if (!permission.IsEnabled)
        {
            return false;
        }

        if (!await _stateCheckerManager.IsEnabledAsync(permission))
        {
            return false;
        }

        var isGranted = false;
        var context = new PermissionValueCheckContext(permission, claimsPrincipal);
        foreach (var provider in _permissionValueProviderManager.ValueProviders)
        {
            if (context.Permission.AllowedProviders.Any() && !context.Permission.AllowedProviders.Contains(provider!.Name))
            {
                continue;
            }

            var result = await provider!.CheckAsync(context);

            if (result == PermissionGrantResult.Granted)
            {
                isGranted = true;
            }
            else if (result == PermissionGrantResult.Prohibited)
            {
                return false;
            }
        }

        return isGranted;
    }

    public async Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names)
    {
        return await IsGrantedAsync(_principalAccessor.Principal, names);
    }

    public async Task<MultiplePermissionGrantResult> IsGrantedAsync(ClaimsPrincipal? claimsPrincipal, string[] names)
    {
        var result = new MultiplePermissionGrantResult();
        if (!names.Any())
        {
            return result;
        }

        var permissionDefinitions = new List<PermissionDefinition>();
        foreach (var name in names)
        {
            var permission = await _permissionDefinitionManager.GetAsync(name);

            result.Result.Add(name, PermissionGrantResult.Undefined);

            if (!permission.IsEnabled)
            {
                continue;
            }
            var permissionState = await _stateCheckerManager.IsEnabledAsync(permission);
            if (!permissionState)
            {
                continue;
            }
            permissionDefinitions.Add(permission);
        }

        foreach (var provider in _permissionValueProviderManager.ValueProviders)
        {
            var permissions = permissionDefinitions
                .Where(x => !x.AllowedProviders.Any() || x.AllowedProviders.Contains(provider!.Name))
                .ToList();

            if (permissions.IsNullOrEmpty())
            {
                break;
            }

            var context = new PermissionValuesCheckContext(permissions, claimsPrincipal);

            var multipleResult = await provider!.CheckAsync(context);
            foreach (var grantResult in multipleResult.Result.Where(grantResult =>
                result.Result.ContainsKey(grantResult.Key) &&
                result.Result[grantResult.Key] == PermissionGrantResult.Undefined &&
                grantResult.Value != PermissionGrantResult.Undefined))
            {
                result.Result[grantResult.Key] = grantResult.Value;
                permissionDefinitions.RemoveAll(x => x.Name == grantResult.Key);
            }

            if (result.AllGranted || result.AllProhibited)
            {
                break;
            }
        }

        return result;
    }
}
