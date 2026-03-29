// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Permissions.Abstractions;
using Skywalker.Security.Claims;

namespace Skywalker.Permissions;

public class RolePermissionValueProvider : PermissionValueProvider
{
    public const string ProviderName = "R";

    public override string Name => ProviderName;

    public RolePermissionValueProvider(IPermissionValidator permissionStore)
        : base(permissionStore)
    {

    }

    public override async Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context)
    {
        var roles = context.Principal?.FindAll(SkywalkerClaimTypes.Role).Select(c => c.Value).ToArray();

        if (roles == null || !roles.Any())
        {
            return PermissionGrantResult.Undefined;
        }

        foreach (var role in roles.Distinct())
        {
            if (await Validator.IsGrantedAsync(context.Permission.Name, Name, role))
            {
                return PermissionGrantResult.Granted;
            }
        }

        return PermissionGrantResult.Undefined;
    }

    public override async Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context)
    {
        var permissionNames = context.Permissions.Select(x => x.Name).Distinct().ToList();
        Check.NotNullOrEmpty(permissionNames, nameof(permissionNames));

        var result = new MultiplePermissionGrantResult(permissionNames.ToArray());

        var roles = context.Principal?.FindAll(SkywalkerClaimTypes.Role).Select(c => c.Value).ToArray();
        if (roles == null || !roles.Any())
        {
            return result;
        }

        foreach (var role in roles.Distinct())
        {
            var multipleResult = await Validator.IsGrantedAsync(permissionNames.ToArray(), Name, role);

            foreach (var grantResult in multipleResult.Result.Where(grantResult =>
                result.Result.ContainsKey(grantResult.Key) &&
                result.Result[grantResult.Key] == PermissionGrantResult.Undefined &&
                grantResult.Value != PermissionGrantResult.Undefined))
            {
                result.Result[grantResult.Key] = grantResult.Value;
                permissionNames.RemoveAll(x => x == grantResult.Key);
            }

            if (result.AllGranted || result.AllProhibited)
            {
                break;
            }

            if (permissionNames.IsNullOrEmpty())
            {
                break;
            }
        }

        return result;
    }
}
