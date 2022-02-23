using Microsoft.Extensions.DependencyInjection;
using Skywalker.Extensions.SimpleStateChecking;

namespace Skywalker.AspNetCore.Authorization.Permissions;

public class RequirePermissionsSimpleBatchStateChecker<TState> : SimpleBatchStateCheckerBase<TState>
    where TState : IHasSimpleStateCheckers<TState>
{
    public static RequirePermissionsSimpleBatchStateChecker<TState> Current => s_current.Value!;
    private static readonly AsyncLocal<RequirePermissionsSimpleBatchStateChecker<TState>> s_current = new();

    private readonly List<RequirePermissionsSimpleBatchStateCheckerModel<TState>> _models;

    static RequirePermissionsSimpleBatchStateChecker()
    {
        s_current.Value = new RequirePermissionsSimpleBatchStateChecker<TState>();
    }

    public RequirePermissionsSimpleBatchStateChecker()
    {
        _models = new List<RequirePermissionsSimpleBatchStateCheckerModel<TState>>();
    }

    public RequirePermissionsSimpleBatchStateChecker<TState> AddCheckModels(params RequirePermissionsSimpleBatchStateCheckerModel<TState>[] models)
    {
        Check.NotNullOrEmpty(models, nameof(models));

        _models.AddRange(models);
        return this;
    }

    public static IDisposable Use(RequirePermissionsSimpleBatchStateChecker<TState> checker)
    {
        var previousValue = Current;
        s_current.Value = checker;
        return new DisposeAction(() => s_current.Value = previousValue);
    }

    public override async Task<SimpleStateCheckerResult<TState>> IsEnabledAsync(SimpleBatchStateCheckerContext<TState> context)
    {
        var permissionChecker = context.ServiceProvider.GetRequiredService<IPermissionChecker>();

        var result = new SimpleStateCheckerResult<TState>(context.States);

        var permissions = _models.Where(x => context.States.Any(s => s.Equals(x.State))).SelectMany(x => x.Permissions).Distinct().ToArray();
        var grantResult = await permissionChecker.IsGrantedAsync(permissions);

        foreach (var state in context.States)
        {
            var model = _models.FirstOrDefault(x => x.State.Equals(state));
            if (model != null)
            {
                if (model.RequiresAll)
                {
                    result[model.State] = model.Permissions.All(x => grantResult.Result.Any(y => y.Key == x && y.Value == PermissionGrantResult.Granted));
                }
                else
                {
                    result[model.State] = grantResult.Result.Any(x => model.Permissions.Contains(x.Key) && x.Value == PermissionGrantResult.Granted);
                }
            }
        }

        return result;
    }
}
