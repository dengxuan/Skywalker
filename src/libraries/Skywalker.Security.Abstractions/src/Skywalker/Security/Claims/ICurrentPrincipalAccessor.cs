using System.Security.Claims;

namespace Skywalker.Security.Claims;

/// <summary>
/// 
/// </summary>
public interface ICurrentPrincipalAccessor
{
    /// <summary>
    /// 
    /// </summary>
    ClaimsPrincipal? Principal { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    IDisposable Change(ClaimsPrincipal principal);
}
