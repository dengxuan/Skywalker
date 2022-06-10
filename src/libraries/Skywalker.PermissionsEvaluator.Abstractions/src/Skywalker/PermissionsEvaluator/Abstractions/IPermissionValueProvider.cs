using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.PermissionsEvaluator.Abstractions;

public interface IPermissionValueProvider : ITransientDependency
{
    string Name { get; }

    //TODO: Rename to GetResult? (CheckAsync throws exception by naming convention)
    Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context);

    Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context);
}
