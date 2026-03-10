// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Permissions;

public class PermissionStateContext
{
    public IServiceProvider? ServiceProvider { get; set; }

    public PermissionDefinition? Permission { get; set; }
}
