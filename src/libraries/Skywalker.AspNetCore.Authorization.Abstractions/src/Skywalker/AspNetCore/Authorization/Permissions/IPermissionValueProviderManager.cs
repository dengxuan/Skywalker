using System.Collections.Generic;

namespace Skywalker.Authorization.Permissions;

public interface IPermissionValueProviderManager
{
    IReadOnlyList<IPermissionValueProvider> ValueProviders { get; }
}
