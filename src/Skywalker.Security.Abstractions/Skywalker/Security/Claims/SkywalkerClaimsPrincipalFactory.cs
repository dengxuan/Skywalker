// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skywalker.DependencyInjection;

namespace Skywalker.Security.Claims;

/// <summary>
/// Claims 主体工厂实现类。
/// </summary>
public class SkywalkerClaimsPrincipalFactory(IServiceScopeFactory serviceScopeFactory, IOptions<SkywalkerClaimsPrincipalFactoryOptions> abpClaimOptions) : ISkywalkerClaimsPrincipalFactory, ITransientDependency
{
    public static string AuthenticationType => "Skywalker.Application";

    protected IServiceScopeFactory ServiceScopeFactory { get; } = serviceScopeFactory;

    protected SkywalkerClaimsPrincipalFactoryOptions Options { get; } = abpClaimOptions.Value;

    public virtual async Task<ClaimsPrincipal> InternalCreateAsync(SkywalkerClaimsPrincipalFactoryOptions options, ClaimsPrincipal? existsClaimsPrincipal = null, bool isDynamic = false)
    {
        var claimsPrincipal = existsClaimsPrincipal ?? new ClaimsPrincipal(new ClaimsIdentity(
            AuthenticationType,
            SkywalkerClaimTypes.UserName,
            SkywalkerClaimTypes.Role));

        using var scope = ServiceScopeFactory.CreateScope();

        var context = new SkywalkerClaimsPrincipalContributorContext(claimsPrincipal, scope.ServiceProvider);

        if (!isDynamic)
        {
            foreach (var contributorType in options.Contributors)
            {
                var contributor = (ISkywalkerClaimsPrincipalContributor)scope.ServiceProvider.GetRequiredService(contributorType);
                await contributor.ContributeAsync(context);
            }
        }
        else
        {
            foreach (var contributorType in options.DynamicContributors)
            {
                var contributor = (ISkywalkerClaimsPrincipalContributor)scope.ServiceProvider.GetRequiredService(contributorType);
                await contributor.ContributeAsync(context);
            }
        }

        return context.ClaimsPrincipal;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="existsClaimsPrincipal"></param>
    /// <returns></returns>
    public virtual Task<ClaimsPrincipal> CreateAsync(ClaimsPrincipal? existsClaimsPrincipal = null) => InternalCreateAsync(Options, existsClaimsPrincipal, false);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="existsClaimsPrincipal"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual Task<ClaimsPrincipal> CreateDynamicAsync(ClaimsPrincipal? existsClaimsPrincipal = null) => throw new NotImplementedException();
}
