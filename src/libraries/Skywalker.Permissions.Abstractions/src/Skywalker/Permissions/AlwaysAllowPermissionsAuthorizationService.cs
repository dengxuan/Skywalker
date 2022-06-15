// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Permissions;

public class AlwaysAllowPermissionsAuthorizationService : IPermissionsAuthorizationService
{
    public Task CheckAsync(PermissionsAuthorizationContext context)
    {
        return Task.CompletedTask;
    }
}
