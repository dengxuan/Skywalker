// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Skywalker.Settings.Abstractions;
using Skywalker.Settings.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for mapping setting endpoints.
/// </summary>
public static class SettingEndpointRouteBuilderExtensions
{
    /// <summary>
    /// Maps the setting endpoints with a custom route prefix.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <param name="routePrefix">The route prefix (e.g., "api/settings", "admin/settings").</param>
    /// <returns>The route group builder for further customization.</returns>
    public static RouteGroupBuilder MapSettingEndpoints(this IEndpointRouteBuilder endpoints, string routePrefix = "api/settings")
    {
        var group = endpoints.MapGroup(routePrefix)
            .WithTags("Settings");

        // GET: Get all settings
        group.MapGet("/", async (ISettingProvider settingProvider) =>
        {
            var settings = await settingProvider.GetAllAsync();
            var result = settings.Select(s => new SettingDto(s.Name, s.Value)).ToList();
            return TypedResults.Ok(result);
        })
        .WithName("GetAllSettings")
        .WithSummary("Get all settings");

        // GET: Get a specific setting
        group.MapGet("/{name}", async (string name, ISettingProvider settingProvider) =>
        {
            var value = await settingProvider.GetOrNullAsync(name);
            if (value == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(new SettingDto(name, value));
        })
        .WithName("GetSetting")
        .WithSummary("Get a specific setting value");

        // PUT: Create or update a setting
        group.MapPut("/{name}", async (string name, UpdateSettingDto input, ISettingManager settingManager) =>
        {
            await settingManager.SetAsync(name, input.Value, input.ProviderName, input.ProviderKey);
            return TypedResults.NoContent();
        })
        .WithName("SetSetting")
        .WithSummary("Create or update a setting");

        // DELETE: Delete a setting
        group.MapDelete("/{name}", async (
            string name,
            string providerName,
            string? providerKey,
            ISettingManager settingManager) =>
        {
            await settingManager.DeleteAsync(name, providerName, providerKey);
            return TypedResults.NoContent();
        })
        .WithName("DeleteSetting")
        .WithSummary("Delete a setting");

        return group;
    }
}
