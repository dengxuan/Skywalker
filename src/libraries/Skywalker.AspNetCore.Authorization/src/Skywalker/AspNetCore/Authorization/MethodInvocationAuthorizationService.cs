﻿using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Skywalker.Authorization;

namespace Skywalker.AspNetCore.Authorization;

public class MethodInvocationAuthorizationService : IMethodInvocationAuthorizationService
{
    private readonly ISkywalkerAuthorizationPolicyProvider _authorizationPolicyProvider;
    private readonly ISkywalkerAuthorizationService _authorizationService;

    public MethodInvocationAuthorizationService(ISkywalkerAuthorizationPolicyProvider abpAuthorizationPolicyProvider, ISkywalkerAuthorizationService abpAuthorizationService)
    {
        _authorizationPolicyProvider = abpAuthorizationPolicyProvider;
        _authorizationService = abpAuthorizationService;
    }

    public async Task CheckAsync(MethodInvocationAuthorizationContext context)
    {
        if (AllowAnonymous(context))
        {
            return;
        }

        var authorizationPolicy = await AuthorizationPolicy.CombineAsync(
            _authorizationPolicyProvider,
            GetAuthorizationDataAttributes(context.Method)
        );

        if (authorizationPolicy == null)
        {
            return;
        }

        await _authorizationService.CheckAsync(authorizationPolicy);
    }

    protected virtual bool AllowAnonymous(MethodInvocationAuthorizationContext context)
    {
        return context.Method.GetCustomAttributes(true).OfType<IAllowAnonymous>().Any();
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
}
