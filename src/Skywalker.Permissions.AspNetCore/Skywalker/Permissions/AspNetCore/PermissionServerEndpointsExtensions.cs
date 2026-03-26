// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions.AspNetCore;

/// <summary>
/// 权限服务端点扩展
/// </summary>
public static class PermissionServerEndpointsExtensions
{
    /// <summary>
    /// 映射权限服务端点
    /// </summary>
    public static IEndpointRouteBuilder MapPermissionsServer(this IEndpointRouteBuilder endpoints)
    {
        var options = endpoints.ServiceProvider.GetRequiredService<IOptions<PermissionServerOptions>>().Value;

        // Discovery 端点
        endpoints.MapGet(options.DiscoveryEndpoint, (IOptions<PermissionServerOptions> opts) =>
        {
            var config = new PermissionsConfiguration
            {
                PermissionGrantsEndpoint = opts.Value.PermissionGrantsEndpoint,
                RegisterEndpoint = opts.Value.RegisterEndpoint
            };
            return Results.Ok(config);
        });

        // 权限授权同步端点
        endpoints.MapGet(options.PermissionGrantsEndpoint, async (
            HttpContext context,
            IPermissionGrantStore store) =>
        {
            var version = await store.GetVersionAsync(context.RequestAborted);
            var etag = $"\"{version}\"";

            // 检查 If-None-Match
            if (context.Request.Headers.IfNoneMatch.ToString() == etag)
            {
                return Results.StatusCode(304);
            }

            context.Response.Headers.ETag = etag;

            // HEAD 请求只返回 ETag
            if (HttpMethods.IsHead(context.Request.Method))
            {
                return Results.Ok();
            }

            var grants = await store.GetAllAsync(context.RequestAborted);
            return Results.Ok(grants);
        });

        // HEAD 请求支持
        endpoints.MapMethods(options.PermissionGrantsEndpoint, ["HEAD"], async (
            HttpContext context,
            IPermissionGrantStore store) =>
        {
            var version = await store.GetVersionAsync(context.RequestAborted);
            context.Response.Headers.ETag = $"\"{version}\"";
            return Results.Ok();
        });

        // 权限注册端点
        endpoints.MapPost(options.RegisterEndpoint, async (
            PermissionRegisterRequest request,
            HttpContext context,
            IPermissionDefinitionManager manager,
            IOptions<PermissionServerOptions> opts) =>
        {
            // 验证 X-Secret
            var secret = context.Request.Headers["X-Secret"].ToString();
            if (string.IsNullOrEmpty(secret))
            {
                return Results.Unauthorized();
            }

            // 验证密钥（简化版，实际应该根据请求来源验证）
            if (!opts.Value.Secrets.Values.Contains(secret))
            {
                return Results.Unauthorized();
            }

            await manager.CreatePermissionsAsync(request.Permissions.ToList());
            return Results.Created();
        });

        return endpoints;
    }
}

/// <summary>
/// 权限注册请求
/// </summary>
public class PermissionRegisterRequest
{
    /// <summary>
    /// 权限定义列表
    /// </summary>
    public required IEnumerable<PermissionDefinition> Permissions { get; set; }
}
