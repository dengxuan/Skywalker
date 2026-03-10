// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Permissions;

/// <summary>
/// 
/// </summary>
public class AlwaysAllowPermissionsAuthorizationService : IPermissionsAuthorizationService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public Task CheckAsync(PermissionsAuthorizationContext context)
    {
        return Task.CompletedTask;
    }
}
