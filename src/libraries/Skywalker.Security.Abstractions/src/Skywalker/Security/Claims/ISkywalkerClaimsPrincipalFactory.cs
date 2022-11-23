// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Security.Claims;

namespace Skywalker.Security.Claims;

/// <summary>
/// 
/// </summary>
public interface ISkywalkerClaimsPrincipalFactory
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="existsClaimsPrincipal"></param>
    /// <returns></returns>
    Task<ClaimsPrincipal> CreateAsync(ClaimsPrincipal? existsClaimsPrincipal = null);
}
