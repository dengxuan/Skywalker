using System.Security.Claims;
using System.Security.Principal;
using Skywalker.Extensions.DependencyInjection;
using Skywalker.Security.Claims;

namespace Skywalker.Security.Users;

public class CurrentUser : ICurrentUser, ITransientDependency
{
    private static readonly Claim[] s_emptyClaimsArray = Array.Empty<Claim>();

    private readonly ICurrentPrincipalAccessor _principalAccessor;

    public virtual bool IsAuthenticated => _principalAccessor.Principal?.Identity?.IsAuthenticated == true;

    public virtual long? Id => _principalAccessor.Principal?.FindUserId();

    public virtual string? Username => this.FindClaimValue(SkywalkerClaimTypes.Username);

    public virtual string? PhoneNumber => this.FindClaimValue(SkywalkerClaimTypes.PhoneNumber);

    public virtual string? Email => this.FindClaimValue(SkywalkerClaimTypes.Email);

    public virtual string[] Roles => FindClaims(SkywalkerClaimTypes.Role).Select(c => c.Value).ToArray();

    public CurrentUser(ICurrentPrincipalAccessor principalAccessor)
    {
        _principalAccessor = principalAccessor;
    }

    public virtual Claim? FindClaim(string claimType)
    {
        return _principalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == claimType);
    }

    public virtual Claim[] FindClaims(string claimType)
    {
        return _principalAccessor.Principal?.Claims.Where(c => c.Type == claimType).ToArray() ?? s_emptyClaimsArray;
    }

    public virtual Claim[] GetAllClaims()
    {
        return _principalAccessor.Principal?.Claims.ToArray() ?? s_emptyClaimsArray;
    }

    public virtual bool IsInRole(string roleName)
    {
        return FindClaims(SkywalkerClaimTypes.Role).Any(c => c.Value == roleName);
    }
}
