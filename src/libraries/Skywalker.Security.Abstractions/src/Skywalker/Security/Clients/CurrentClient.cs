// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Security.Principal;
using Skywalker.Security.Claims;

namespace Skywalker.Security.Clients;

public class CurrentClient : ICurrentClient
{
    public virtual string? Id => _principalAccessor?.Principal?.FindClientId();

    public virtual bool IsAuthenticated => Id != null;

    private readonly ICurrentPrincipalAccessor _principalAccessor;

    public CurrentClient(ICurrentPrincipalAccessor principalAccessor)
    {
        _principalAccessor = principalAccessor;
    }
}
