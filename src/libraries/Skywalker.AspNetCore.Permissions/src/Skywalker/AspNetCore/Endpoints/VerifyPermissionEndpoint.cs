// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Skywalker.Permissions.Abstractions;

namespace Skywalker.AspNetCore.Endpoints;

public class VerifyPermissionEndpoint : IEndpointHandler
{
    private readonly IPermissionValidator _permissionValidator;

    private readonly ILogger<VerifyPermissionEndpoint> _logger;

    public VerifyPermissionEndpoint(IPermissionValidator permissionValidator, ILogger<VerifyPermissionEndpoint> logger)
    {
        _permissionValidator = permissionValidator;
        _logger = logger;
    }

    public async Task<IEndpointResult> ProcessAsync(HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated == false)
        {
            _logger.LogWarning("Invalid HTTP method for checkpermission endpoint. User is not authenticated.");
            return new StatusCodeResult(HttpStatusCode.Unauthorized);
        }
        if (!HttpMethods.IsGet(context.Request.Method))
        {
            _logger.LogWarning("Invalid HTTP method for checkpermission endpoint. Only GET is allowed.");
            return new StatusCodeResult(HttpStatusCode.MethodNotAllowed);
        }
        var name = context.Request.Query["name"].ToString();
        var providerName = context.Request.Query["providerName"].ToString();
        var providerKey = context.Request.Query["providerKey"].ToString();
        if (name.IsNullOrEmpty() || providerName.IsNullOrEmpty() || providerKey.IsNullOrEmpty())
        {
            _logger.LogWarning("Invalid HTTP method for checkpermission endpoint. Missing name,providerName,providerKey query parameter.");
            return new StatusCodeResult(HttpStatusCode.BadRequest);
        }
        var isGranted = await _permissionValidator.IsGrantedAsync(name, providerName, providerKey);
        _logger.LogInformation("{name} is {isGranted}", name, isGranted ? "granted" : "denied");
        return new StatusCodeResult(isGranted ? HttpStatusCode.NoContent : HttpStatusCode.Unauthorized);
    }
}
