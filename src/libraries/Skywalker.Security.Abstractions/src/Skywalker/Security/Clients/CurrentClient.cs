// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Security.Principal;
using Skywalker.Extensions.DependencyInjection;
using Skywalker.Security.Claims;

namespace Skywalker.Security.Clients;

public class CurrentClient : ICurrentClient, ITransientDependency
{
    public virtual bool IsAuthenticated => _principalAccessor.Principal?.Identity?.IsAuthenticated == true;

    public virtual string? Id => _principalAccessor?.Principal?.FindClientId();

    private readonly ICurrentPrincipalAccessor _principalAccessor;

    public CurrentClient(ICurrentPrincipalAccessor principalAccessor)
    {
        _principalAccessor = principalAccessor;
    }
}
