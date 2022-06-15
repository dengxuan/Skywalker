// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Security.Claims;

namespace Skywalker.Permissions.Abstractions;

public interface IPermissionChecker
{
    Task<bool> IsGrantedAsync(string name);

    Task<bool> IsGrantedAsync(ClaimsPrincipal? claimsPrincipal, string name);

    Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names);

    Task<MultiplePermissionGrantResult> IsGrantedAsync(ClaimsPrincipal? claimsPrincipal, string[] names);
}
