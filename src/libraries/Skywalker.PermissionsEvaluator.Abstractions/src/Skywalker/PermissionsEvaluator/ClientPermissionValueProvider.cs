﻿using Skywalker.PermissionsEvaluator.Abstractions;
using Skywalker.Security.Claims;

namespace Skywalker.PermissionsEvaluator;

public class ClientPermissionValueProvider : PermissionValueProvider
{
    public const string ProviderName = "C";

    public override string Name => ProviderName;

    public ClientPermissionValueProvider(IPermissionValidator permissionStore) : base(permissionStore)
    {
    }

    public override async Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context)
    {
        var clientId = context.Principal?.FindFirst(SkywalkerClaimTypes.ClientId)?.Value;

        if (clientId == null)
        {
            return PermissionGrantResult.Undefined;
        }
        return await Validator.IsGrantedAsync(context.Permission.Name, Name, clientId)
            ? PermissionGrantResult.Granted
            : PermissionGrantResult.Undefined;
    }

    public override async Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context)
    {
        var permissionNames = context.Permissions.Select(x => x.Name).Distinct().ToArray();
        Check.NotNullOrEmpty(permissionNames, nameof(permissionNames));

        var clientId = context.Principal?.FindFirst(SkywalkerClaimTypes.ClientId)?.Value;
        if (clientId == null)
        {
            return new MultiplePermissionGrantResult(permissionNames); ;
        }

        return await Validator.IsGrantedAsync(permissionNames, Name, clientId);
    }
}
