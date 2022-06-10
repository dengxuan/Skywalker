using Skywalker.PermissionsEvaluator.Abstractions;
using Skywalker.Security.Claims;

namespace Skywalker.PermissionsEvaluator;

public class UserPermissionValueProvider : PermissionValueProvider
{
    public const string ProviderName = "U";

    public override string Name => ProviderName;

    public UserPermissionValueProvider(IPermissionValidator permissionStore)
        : base(permissionStore)
    {

    }

    public override async Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context)
    {
        var userId = context.Principal?.FindFirst(SkywalkerClaimTypes.UserId)?.Value;

        if (userId == null)
        {
            return PermissionGrantResult.Undefined;
        }

        return await Validator.IsGrantedAsync(context.Permission.Name, Name, userId)
            ? PermissionGrantResult.Granted
            : PermissionGrantResult.Undefined;
    }

    public override async Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context)
    {
        var permissionNames = context.Permissions.Select(x => x.Name).Distinct().ToArray();
        Check.NotNullOrEmpty(permissionNames, nameof(permissionNames));

        var userId = context.Principal?.FindFirst(SkywalkerClaimTypes.UserId)?.Value;
        if (userId == null)
        {
            return new MultiplePermissionGrantResult(permissionNames);
        }

        return await Validator.IsGrantedAsync(permissionNames, Name, userId);
    }
}
