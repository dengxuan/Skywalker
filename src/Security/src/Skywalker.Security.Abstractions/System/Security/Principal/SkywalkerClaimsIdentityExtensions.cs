﻿using Skywalker;
using Skywalker.Security.Claims;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;

namespace System.Security.Principal
{
    public static class SkywalkerClaimsIdentityExtensions
    {
        public static Guid? FindUserId([NotNull] this ClaimsPrincipal principal)
        {
            Check.NotNull(principal, nameof(principal));

            var userIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == SkywalkerClaimTypes.UserId);
            if (userIdOrNull == null || userIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }
            if (Guid.TryParse(userIdOrNull.Value, out Guid result))
            {
                return result;
            }
            return null;
        }

        public static Guid? FindUserId([NotNull] this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));

            var claimsIdentity = identity as ClaimsIdentity;

            var userIdOrNull = claimsIdentity?.Claims?.FirstOrDefault(c => c.Type == SkywalkerClaimTypes.UserId);
            if (userIdOrNull == null || userIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return Guid.Parse(userIdOrNull.Value);
        }

        public static string? FindClientId([NotNull] this ClaimsPrincipal principal)
        {
            Check.NotNull(principal, nameof(principal));

            var clientIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == SkywalkerClaimTypes.ClientId);
            if (clientIdOrNull == null || clientIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return clientIdOrNull.Value;
        }

        public static string? FindClientId([NotNull] this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));

            var claimsIdentity = identity as ClaimsIdentity;

            var clientIdOrNull = claimsIdentity?.Claims?.FirstOrDefault(c => c.Type == SkywalkerClaimTypes.ClientId);
            if (clientIdOrNull == null || clientIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return clientIdOrNull.Value;
        }
    }
}
