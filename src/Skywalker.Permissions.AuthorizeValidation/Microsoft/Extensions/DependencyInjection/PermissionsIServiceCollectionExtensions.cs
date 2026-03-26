// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Skywalker.Permissions;
using Skywalker.Permissions.Abstractions;
using Skywalker.Permissions.AuthorizeValidation;

namespace Microsoft.Extensions.DependencyInjection;

public static class PermissionsIServiceCollectionExtensions
{
    public static IServiceCollection AddPermissionValidation(this IServiceCollection services, Action<PermissionValidationOptions> options)
    {
        services.Configure(options);

        // 注册内存缓存（如果尚未注册）
        services.AddMemoryCache();

        // 注册权限验证器（基于内存缓存）
        services.TryAddSingleton<IPermissionValidator, InMemoryPermissionValidator>();

        // 注册 HttpClient
        services.AddHttpClient(Constants.PermissionHttpClientName, (serviceProvider, httpClient) =>
        {
            var validationOptions = serviceProvider.GetRequiredService<IOptions<PermissionValidationOptions>>();
            httpClient.BaseAddress = new Uri(validationOptions.Value.Authority);

            // 添加服务间认证头
            if (!string.IsNullOrEmpty(validationOptions.Value.Secret))
            {
                httpClient.DefaultRequestHeaders.Add("X-Secret", validationOptions.Value.Secret);
            }
        });

        // 注册后台同步服务
        services.AddHostedService<PermissionSyncHostedService>();

        return services;
    }
}
