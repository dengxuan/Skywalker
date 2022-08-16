﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Skywalker.Extensions.DependencyInjection.Abstractions;

namespace Skywalker.AspNetCore.Permissions.Abstractions;

public interface ISkywalkerAuthorizationService : IAuthorizationService, IServiceProviderAccessor
{
    ClaimsPrincipal? CurrentPrincipal { get; }
}