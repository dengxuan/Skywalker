namespace Skywalker.AspNetCore.Authorization.Permissions;

public interface IPermissionValueProviderManager
{
    IReadOnlyList<IPermissionValueProvider?> ValueProviders { get; }
}
