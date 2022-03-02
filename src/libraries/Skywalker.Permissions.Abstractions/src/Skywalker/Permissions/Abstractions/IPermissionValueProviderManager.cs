namespace Skywalker.Permissions.Abstractions;

public interface IPermissionValueProviderManager
{
    IReadOnlyList<IPermissionValueProvider?> ValueProviders { get; }
}
