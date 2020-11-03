using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Skywalker.Users
{
    public interface ICurrentUser
    {
        bool IsAuthenticated { get; }

        [MaybeNull]
        Guid? Id { get; }

        [MaybeNull]
        string? Username { get; }

        [MaybeNull]
        string? PhoneNumber { get; }

        [MaybeNull]
        string? Email { get; }

        [NotNull]
        string[] Roles { get; }

        Claim? FindClaim(string claimType);

        Claim[] FindClaims(string claimType);

        Claim[] GetAllClaims();

        bool IsInRole(string roleName);
    }
}
