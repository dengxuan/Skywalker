namespace Skywalker.Authorization.Permissions.Abstractions;

public interface IPermissionValueProviderManager
{
    IReadOnlyList<IPermissionValueProvider?> ValueProviders { get; }
}
