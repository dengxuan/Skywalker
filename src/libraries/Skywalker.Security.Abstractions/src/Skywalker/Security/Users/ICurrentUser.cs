using System.Security.Claims;

namespace Skywalker.Security.Users;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    Guid? Id { get; }

    string? Username { get; }

    string? PhoneNumber { get; }

    string? Email { get; }

    string[] Roles { get; }

    Claim? FindClaim(string claimType);

    Claim[] FindClaims(string claimType);

    Claim[] GetAllClaims();

    bool IsInRole(string roleName);
}
