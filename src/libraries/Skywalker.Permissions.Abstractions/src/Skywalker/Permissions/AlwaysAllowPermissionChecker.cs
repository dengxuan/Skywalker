// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Security.Claims;
using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions;

/// <summary>
/// Always allows for any permission.
///
/// Use IServiceCollection.AddAlwaysAllowPermissionsEvaluator() to replace
/// IPermissionChecker with this class. This is useful for tests.
/// </summary>
public class AlwaysAllowPermissionChecker : IPermissionChecker
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task<bool> IsGrantedAsync(string name)
    {
        return TaskCache.TrueResult;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task<bool> IsGrantedAsync(ClaimsPrincipal? claimsPrincipal, string name)
    {
        return TaskCache.TrueResult;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    public Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names)
    {
        return IsGrantedAsync(null, names);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <param name="names"></param>
    /// <returns></returns>
    public Task<MultiplePermissionGrantResult> IsGrantedAsync(ClaimsPrincipal? claimsPrincipal, string[] names)
    {
        return Task.FromResult(new MultiplePermissionGrantResult(names, PermissionGrantResult.Granted));
    }
}
