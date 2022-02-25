using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.AspNetCore.Authorization;

public interface ISkywalkerAuthorizationService : IAuthorizationService, IServiceProviderAccessor
{
    ClaimsPrincipal? CurrentPrincipal { get; }
}
