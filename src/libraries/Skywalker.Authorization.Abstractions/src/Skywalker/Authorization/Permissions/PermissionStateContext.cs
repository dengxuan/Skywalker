using System;

namespace Skywalker.Authorization.Permissions;

public class PermissionStateContext
{
    public IServiceProvider? ServiceProvider { get; set; }

    public PermissionDefinition? Permission { get; set; }
}
