// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Skywalker.Extensions.DependencyInjection;
using Skywalker.Security.Claims;

namespace Skywalker.AspNetCore;

public class HttpContextCurrentPrincipalAccessor : ThreadCurrentPrincipalAccessor, ISingletonDependency
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextCurrentPrincipalAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override ClaimsPrincipal? GetClaimsPrincipal()
    {
        return _httpContextAccessor.HttpContext?.User ?? base.GetClaimsPrincipal();
    }
}
