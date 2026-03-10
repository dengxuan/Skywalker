using System.Security.Claims;
using Skywalker.DependencyInjection;

namespace Skywalker.Security.Claims;

/// <summary>
/// 基于线程的当前主体访问器实现类。
/// </summary>
[ExposeServices(typeof(ICurrentPrincipalAccessor))]
public class ThreadCurrentPrincipalAccessor : CurrentPrincipalAccessor, ISingletonDependency
{
    protected override ClaimsPrincipal? GetClaimsPrincipal()
    {
        return Thread.CurrentPrincipal as ClaimsPrincipal;
    }
}
