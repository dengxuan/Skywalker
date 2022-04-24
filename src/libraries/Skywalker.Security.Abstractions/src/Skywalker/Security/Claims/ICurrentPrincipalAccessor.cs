using System.Security.Claims;
using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Security.Claims;

public interface ICurrentPrincipalAccessor: ISingletonDependency
{
    ClaimsPrincipal? Principal { get; }

    IDisposable Change(ClaimsPrincipal principal);
}
