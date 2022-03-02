using Microsoft.Extensions.DependencyInjection;
using Skywalker.Extensions.SimpleStateChecking;
using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions;

public class RequirePermissionsSimpleStateChecker<TState> : ISimpleStateChecker<TState>
    where TState : IHasSimpleStateCheckers<TState>
{
    private readonly RequirePermissionsSimpleBatchStateCheckerModel<TState> _model;

    public RequirePermissionsSimpleStateChecker(RequirePermissionsSimpleBatchStateCheckerModel<TState> model)
    {
        _model = model;
    }

    public async Task<bool> IsEnabledAsync(SimpleStateCheckerContext<TState> context)
    {
        var permissionChecker = context.ServiceProvider.GetRequiredService<IPermissionChecker>();

        if (_model.Permissions.Length == 1)
        {
            return await permissionChecker.IsGrantedAsync(_model.Permissions.First());
        }

        var grantResult = await permissionChecker.IsGrantedAsync(_model.Permissions);

        return _model.RequiresAll
            ? grantResult.AllGranted
            : grantResult.Result.Any(x => _model.Permissions.Contains(x.Key) && x.Value == PermissionGrantResult.Granted);
    }
}
