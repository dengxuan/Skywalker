// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Skywalker.Permissions.Abstractions;

namespace Skywalker.AspNetCore.PermissionsEvaluator.Endpoints;

public class CheckPermissionEndpoint : IEndpointHandler
{
    private readonly IPermissionChecker _permissionChecker;

    private readonly ILogger<CheckPermissionEndpoint> _logger;

    public CheckPermissionEndpoint(IPermissionChecker permissionChecker, ILogger<CheckPermissionEndpoint> logger)
    {
        _permissionChecker = permissionChecker;
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
        var nameValues = context.Request.Query["name"];
        if (nameValues.IsNullOrEmpty())
        {
            _logger.LogWarning("Invalid HTTP method for checkpermission endpoint. Missing name query parameter.");
            return new StatusCodeResult(HttpStatusCode.BadRequest);
        }
        var name = nameValues.ToString();
        var isGranted = await _permissionChecker.IsGrantedAsync(name);
        _logger.LogInformation("{name} is {isGranted}", name, isGranted ? "granted" : "denied");
        return new ChackPermissionResult(name, isGranted);
    }
}
