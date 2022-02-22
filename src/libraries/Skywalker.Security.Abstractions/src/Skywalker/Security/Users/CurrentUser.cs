using System.Security.Claims;
using System.Security.Principal;
using Skywalker.Security.Claims;
using Skywalker.Security.Users;

namespace Skywalker.Users;

public class CurrentUser : ICurrentUser
{
    private static readonly Claim[] EmptyClaimsArray = new Claim[0];

    public virtual bool IsAuthenticated => Id.HasValue;

    public virtual Guid? Id => _principalAccessor.Principal?.FindUserId();

    public virtual string? Username => this.FindClaimValue(SkywalkerClaimTypes.Username);

    public virtual string? PhoneNumber => this.FindClaimValue(SkywalkerClaimTypes.PhoneNumber);

    public virtual string? Email => this.FindClaimValue(SkywalkerClaimTypes.Email);

    public virtual string[] Roles => FindClaims(SkywalkerClaimTypes.Role).Select(c => c.Value).ToArray();

    private readonly ICurrentPrincipalAccessor _principalAccessor;

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
        return _principalAccessor.Principal?.Claims.Where(c => c.Type == claimType).ToArray() ?? EmptyClaimsArray;
    }

    public virtual Claim[] GetAllClaims()
    {
        return _principalAccessor.Principal?.Claims.ToArray() ?? EmptyClaimsArray;
    }

    public virtual bool IsInRole(string roleName)
    {
        return FindClaims(SkywalkerClaimTypes.Role).Any(c => c.Value == roleName);
    }
}
