using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Skywalker.AspNetCore.Permissions.Abstractions;

public interface ISkywalkerAuthorizationService : IAuthorizationService//,IServiceProviderAccessor
{
    ClaimsPrincipal? CurrentPrincipal { get; }
}
