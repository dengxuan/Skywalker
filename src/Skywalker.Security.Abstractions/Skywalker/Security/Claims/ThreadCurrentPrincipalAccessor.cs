using System.Security.Claims;

namespace Skywalker.Security.Claims;

/// <summary>
/// 基于线程的当前主体访问器实现类。
/// </summary>
public class ThreadCurrentPrincipalAccessor : CurrentPrincipalAccessor
{
    protected override ClaimsPrincipal? GetClaimsPrincipal()
    {
        return Thread.CurrentPrincipal as ClaimsPrincipal;
    }
}
