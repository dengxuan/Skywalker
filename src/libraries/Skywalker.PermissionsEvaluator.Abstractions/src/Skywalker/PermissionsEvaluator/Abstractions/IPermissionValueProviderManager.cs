namespace Skywalker.PermissionsEvaluator.Abstractions;

public interface IPermissionValueProviderManager
{
    IReadOnlyList<IPermissionValueProvider?> ValueProviders { get; }
}
