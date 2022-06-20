// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Skywalker.Permissions;
using Skywalker.Permissions.Abstractions;
using System.Text;
using Microsoft.Extensions.Localization;
#if NETCOREAPP3_1_OR_GREATER
using System.Text.Json;
#else
using Newtonsoft.Json;
#endif

namespace Skywalker.AspNetCore.Endpoints;

internal record struct Permission
{
    public string Name { get; init; }

    public string LocalizedStringName { get; init; }

    public bool IsEnabled { get; init; }

    public Dictionary<string, object?> Properties { get; init; }

    public IReadOnlyList<Permission> Children { get; init; }
    
}


internal class PushPermissionsEndpoint : IEndpointHandler
{
    private readonly ILogger<PushPermissionsEndpoint> _logger;

    private readonly IPermissionDefinitionManager _permissionDefinitionManager;

    public PushPermissionsEndpoint(ILogger<PushPermissionsEndpoint> logger, IPermissionDefinitionManager permissionDefinitionManager)
    {
        _logger = logger;
        _permissionDefinitionManager = permissionDefinitionManager;
    }

    public static IReadOnlyList<PermissionDefinition> FromPermissions(List<Permission>? permissions)
    {
        if (permissions is null)
        {
            return new List<PermissionDefinition>();
        }
        var context = new PermissionDefinitionContext();
        foreach (var permission in permissions)
        {
            var diaplsyName = new LocalizedString(permission.LocalizedStringName, permission.LocalizedStringName);
            var definition = context.AddPermission(permission.Name, diaplsyName, permission.IsEnabled);
            foreach (var item in permission.Properties)
            {
                definition[item.Key] = item.Value;
            }
            AddChildren(definition, permission);
        }
        return context.Permissions;

        static void AddChildren(PermissionDefinition definition, Permission permission)
        {
            foreach (var child in permission.Children)
            {
                var diaplsyName = new LocalizedString(child.LocalizedStringName, child.LocalizedStringName);
                var childDefinition = definition.AddChild(child.Name, diaplsyName, child.IsEnabled);
                foreach (var item in permission.Properties)
                {
                    definition[item.Key] = item.Value;
                }
                AddChildren(childDefinition, child);
            }
        }
    }

    public async Task<IEndpointResult> ProcessAsync(HttpContext context)
    {
        if (!HttpMethods.IsPost(context.Request.Method))
        {
            _logger.LogWarning("Invalid HTTP method for commitpermission endpoint. Only POST is allowed.");
            return new StatusCodeResult(HttpStatusCode.MethodNotAllowed);
        }
        var ms = new MemoryStream();
        await context.Request.Body.CopyToAsync(ms);
#if NETCOREAPP3_1_OR_GREATER
        var permissions = await JsonSerializer.DeserializeAsync<List<Permission>>(ms);
#elif NETSTANDARD2_0_OR_GREATER
        var json = Encoding.UTF8.GetString(ms.ToArray());
        var permissions = JsonConvert.DeserializeObject<List<Permission>>(json);
#endif
        var definitions = FromPermissions(permissions);
        await _permissionDefinitionManager.CreatePermissionsAsync(definitions);
        return new StatusCodeResult(HttpStatusCode.NoContent);
    }
}
