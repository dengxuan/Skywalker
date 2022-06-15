using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.AspNetCore.Permissions.Abstractions;
using Skywalker.Security.Claims;

namespace Skywalker.AspNetCore.Permissions;

public class SkywalkerAuthorizationService : DefaultAuthorizationService, ISkywalkerAuthorizationService
{
    public IServiceProvider ServiceProvider { get; }

    public ClaimsPrincipal? CurrentPrincipal => _currentPrincipalAccessor.Principal;

    private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

    public SkywalkerAuthorizationService(
        IAuthorizationPolicyProvider policyProvider,
        IAuthorizationHandlerProvider handlers,
        ILogger<DefaultAuthorizationService> logger,
        IAuthorizationHandlerContextFactory contextFactory,
        IAuthorizationEvaluator evaluator,
        IOptions<AuthorizationOptions> options,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IServiceProvider serviceProvider)
        : base(
            policyProvider,
            handlers,
            logger,
            contextFactory,
            evaluator,
            options)
    {
        _currentPrincipalAccessor = currentPrincipalAccessor;
        ServiceProvider = serviceProvider;
    }
}
