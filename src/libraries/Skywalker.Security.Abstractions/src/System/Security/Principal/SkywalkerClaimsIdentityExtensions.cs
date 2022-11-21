using System.Security.Claims;
using Skywalker.Security.Claims;

namespace System.Security.Principal;

/// <summary>
/// 
/// </summary>
public static class SkywalkerClaimsIdentityExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static long? FindUserId(this ClaimsPrincipal principal)
    {
        Check.NotNull(principal, nameof(principal));

        var userIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == SkywalkerClaimTypes.UserId);
        if (userIdOrNull == null || userIdOrNull.Value.IsNullOrWhiteSpace())
        {
            return null;
        }
        if (long.TryParse(userIdOrNull.Value, out var result))
        {
            return result;
        }
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="identity"></param>
    /// <returns></returns>
    public static long? FindUserId(this IIdentity identity)
    {
        Check.NotNull(identity, nameof(identity));

        var claimsIdentity = identity as ClaimsIdentity;

        var userIdOrNull = claimsIdentity?.Claims?.FirstOrDefault(c => c.Type == SkywalkerClaimTypes.UserId);
        if (userIdOrNull == null || userIdOrNull.Value.IsNullOrWhiteSpace())
        {
            return null;
        }

        if (long.TryParse(userIdOrNull.Value, out var result))
        {
            return result;
        }
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static string? FindClientId(this ClaimsPrincipal principal)
    {
        Check.NotNull(principal, nameof(principal));

        var clientIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == SkywalkerClaimTypes.ClientId);
        if (clientIdOrNull == null || clientIdOrNull.Value.IsNullOrWhiteSpace())
        {
            return null;
        }

        return clientIdOrNull.Value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="identity"></param>
    /// <returns></returns>
    public static string? FindClientId(this IIdentity identity)
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static long? FindTenantcyId(this ClaimsPrincipal principal)
    {

        var tenantIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == SkywalkerClaimTypes.TenantcyId);
        if (tenantIdOrNull == null || tenantIdOrNull.Value.IsNullOrWhiteSpace())
        {
            return null;
        }

        if (long.TryParse(tenantIdOrNull.Value, out var guid))
        {
            return guid;
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="identity"></param>
    /// <returns></returns>
    public static long? FindTenantcyId(this IIdentity identity)
    {

        var claimsIdentity = identity as ClaimsIdentity;

        var tenantIdOrNull = claimsIdentity?.Claims?.FirstOrDefault(c => c.Type == SkywalkerClaimTypes.TenantcyId);
        if (tenantIdOrNull == null || tenantIdOrNull.Value.IsNullOrWhiteSpace())
        {
            return null;
        }

        if (long.TryParse(tenantIdOrNull.Value, out var guid))
        {
            return guid;
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="claimsIdentity"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    public static ClaimsIdentity AddIfNotContains(this ClaimsIdentity claimsIdentity, Claim claim)
    {
        Check.NotNull(claimsIdentity, nameof(claimsIdentity));

        if (!claimsIdentity.Claims.Any(x => string.Equals(x.Type, claim.Type, StringComparison.OrdinalIgnoreCase)))
        {
            claimsIdentity.AddClaim(claim);
        }

        return claimsIdentity;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="claimsIdentity"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    public static ClaimsIdentity AddOrReplace(this ClaimsIdentity claimsIdentity, Claim claim)
    {
        Check.NotNull(claimsIdentity, nameof(claimsIdentity));

        foreach (var x in claimsIdentity.FindAll(claim.Type).ToList())
        {
            claimsIdentity.RemoveClaim(x);
        }

        claimsIdentity.AddClaim(claim);

        return claimsIdentity;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="identity"></param>
    /// <returns></returns>
    public static ClaimsPrincipal AddIdentityIfNotContains(this ClaimsPrincipal principal, ClaimsIdentity identity)
    {
        Check.NotNull(principal, nameof(principal));

        if (!principal.Identities.Any(x => string.Equals(x.AuthenticationType, identity.AuthenticationType, StringComparison.OrdinalIgnoreCase)))
        {
            principal.AddIdentity(identity);
        }

        return principal;
    }
}
