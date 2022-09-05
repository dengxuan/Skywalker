using System.Security.Claims;

namespace Skywalker.Security.Claims;

public class ThreadCurrentPrincipalAccessor : CurrentPrincipalAccessor
{
    protected override ClaimsPrincipal? GetClaimsPrincipal()
    {
        return Thread.CurrentPrincipal as ClaimsPrincipal;
    }
}
