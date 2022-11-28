// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Skywalker.Security.Claims;

internal class SkywalkerClaimsPrincipalFactory : ISkywalkerClaimsPrincipalFactory
{
    public static string AuthenticationType => "Skywalker.Application";

    protected IServiceScopeFactory ServiceScopeFactory { get; }
    protected SkywalkerClaimsPrincipalFactoryOptions Options { get; }

    public SkywalkerClaimsPrincipalFactory(IServiceScopeFactory serviceScopeFactory, IOptions<SkywalkerClaimsPrincipalFactoryOptions> abpClaimOptions)
    {
        ServiceScopeFactory = serviceScopeFactory;
        Options = abpClaimOptions.Value;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="existsClaimsPrincipal"></param>
    /// <returns></returns>
    public virtual async Task<ClaimsPrincipal> CreateAsync(ClaimsPrincipal? existsClaimsPrincipal = null)
    {
        using (var scope = ServiceScopeFactory.CreateScope())
        {
            var claimsPrincipal = existsClaimsPrincipal ?? new ClaimsPrincipal(new ClaimsIdentity(
                AuthenticationType,
                SkywalkerClaimTypes.UserName,
                SkywalkerClaimTypes.Role));

            var context = new SkywalkerClaimsPrincipalContributorContext(claimsPrincipal, scope.ServiceProvider);

            foreach (var contributorType in Options.Contributors)
            {
                var contributor = (ISkywalkerClaimsPrincipalContributor)scope.ServiceProvider.GetRequiredService(contributorType);
                await contributor.ContributeAsync(context);
            }

            return claimsPrincipal;
        }
    }
}
