// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Skywalker.Ddd.Application.Pipeline;

public class AuthorizationPipelineBehavior
{
    private readonly InterceptDelegate _next;
    private readonly ILogger<CachingPipelineBehavior> _logger;
    private readonly IAuthorizationService _authorizationService;
    private readonly IAuthorizationPolicyProvider _authorizationPolicyProvider;

    public AuthorizationPipelineBehavior(ILogger<CachingPipelineBehavior> logger, IAuthorizationService authorizationService, IAuthorizationPolicyProvider authorizationPolicyProvider, InterceptDelegate next)
    {
        _logger = logger;
        _authorizationService = authorizationService;
        _authorizationPolicyProvider = authorizationPolicyProvider;
        _next = next;
    }


    protected virtual bool AllowAnonymous(PipelineContext context)
    {
        return context.Handler.GetType().GetCustomAttributes(true).OfType<IAllowAnonymous>().Any();
    }

    protected virtual IEnumerable<IAuthorizeData> GetAuthorizationDataAttributes(MethodInfo methodInfo)
    {
        var attributes = methodInfo
            .GetCustomAttributes(true)
            .OfType<IAuthorizeData>();

        if (methodInfo.IsPublic && methodInfo.DeclaringType != null)
        {
            attributes = attributes
                .Union(
                    methodInfo.DeclaringType
                        .GetCustomAttributes(true)
                        .OfType<IAuthorizeData>()
                );
        }

        return attributes;
    }

    public async ValueTask InvokeAsync(PipelineContext context)
    {
        _logger.LogInformation("Begin AuthorizationPipelineBehavior");
        if (AllowAnonymous(context))
        {
            await _next(context);
            return;
        }
        var authorizeDatas = GetAuthorizationDataAttributes(context.Handler.GetType().GetMethod("HandleAsync")!);
        var authorizationPolicy = await AuthorizationPolicy.CombineAsync(_authorizationPolicyProvider, authorizeDatas);

        if (authorizationPolicy == null)
        {
            await _next(context);
            return;
        }

        await _authorizationService.CheckAsync(authorizationPolicy);
        await _next(context);
    }
}
