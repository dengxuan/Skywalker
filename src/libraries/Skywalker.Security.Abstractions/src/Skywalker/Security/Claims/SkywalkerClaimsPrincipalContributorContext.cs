// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Security.Claims;

namespace Skywalker.Security.Claims;

public class SkywalkerClaimsPrincipalContributorContext
{
    public ClaimsPrincipal ClaimsPrincipal { get; }

    public IServiceProvider ServiceProvider { get; }

    public SkywalkerClaimsPrincipalContributorContext( ClaimsPrincipal claimsIdentity, IServiceProvider serviceProvider)
    {
        ClaimsPrincipal = claimsIdentity;
        ServiceProvider = serviceProvider;
    }
}
