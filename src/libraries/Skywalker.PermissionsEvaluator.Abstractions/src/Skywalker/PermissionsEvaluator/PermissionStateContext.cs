using System;

namespace Skywalker.PermissionsEvaluator;

public class PermissionStateContext
{
    public IServiceProvider? ServiceProvider { get; set; }

    public PermissionDefinition? Permission { get; set; }
}
