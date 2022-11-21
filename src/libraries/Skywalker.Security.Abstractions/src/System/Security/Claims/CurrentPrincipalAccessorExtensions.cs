// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Security.Claims;

namespace System.Security.Claims;

/// <summary>
/// 
/// </summary>
public static class CurrentPrincipalAccessorExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentPrincipalAccessor"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    public static IDisposable Change(this ICurrentPrincipalAccessor currentPrincipalAccessor, Claim claim)
    {
        return currentPrincipalAccessor.Change(new[] { claim });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentPrincipalAccessor"></param>
    /// <param name="claims"></param>
    /// <returns></returns>
    public static IDisposable Change(this ICurrentPrincipalAccessor currentPrincipalAccessor, IEnumerable<Claim> claims)
    {
        return currentPrincipalAccessor.Change(new ClaimsIdentity(claims));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentPrincipalAccessor"></param>
    /// <param name="claimsIdentity"></param>
    /// <returns></returns>
    public static IDisposable Change(this ICurrentPrincipalAccessor currentPrincipalAccessor, ClaimsIdentity claimsIdentity)
    {
        return currentPrincipalAccessor.Change(new ClaimsPrincipal(claimsIdentity));
    }
}
