// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Skywalker.Permissions;
using Skywalker.Permissions.Abstractions;

namespace Skywalker.AspNetCore.Endpoints;

internal class PullPermissionsEndpoint : IEndpointHandler
{
    private readonly ILogger<PushPermissionsEndpoint> _logger;

    private readonly IPermissionDefinitionManager _permissionDefinitionManager;

    public PullPermissionsEndpoint(ILogger<PushPermissionsEndpoint> logger, IPermissionDefinitionManager permissionDefinitionManager)
    {
        _logger = logger;
        _permissionDefinitionManager = permissionDefinitionManager;
    }

    public static IReadOnlyList<Permission> ToPermissions(IReadOnlyList<PermissionDefinition>? definitions)
    {
        var permissions = new List<Permission>();
        if (definitions is null)
        {
            return permissions;
        }
        foreach (var definition in definitions)
        {
            var permission = new Permission
            {
                Name = definition.Name,
                DisplayName = definition.DisplayName,
                IsEnabled = definition.IsEnabled,
                Properties = definition.Properties,
                Children = ToPermissions(definition.Children)
            };
            permissions.Add(permission);
        }
        return permissions;
    }

    public async Task<IEndpointResult> ProcessAsync(HttpContext context)
    {
        if (!HttpMethods.IsPost(context.Request.Method))
        {
            _logger.LogWarning("Invalid HTTP method for commitpermission endpoint. Only POST is allowed.");
            return new StatusCodeResult(HttpStatusCode.MethodNotAllowed);
        }
        var permissions = await _permissionDefinitionManager.GetPermissionsAsync();
        var definitions = ToPermissions(permissions);
        return new PullPermissionsResult(definitions);
    }
}
