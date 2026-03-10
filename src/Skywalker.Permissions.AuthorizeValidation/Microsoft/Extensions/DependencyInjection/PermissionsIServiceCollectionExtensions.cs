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
    public static IServiceCollection AddAuthorizeValidation(this IServiceCollection services, Action<PermissionValidationOptions> options)
    {
        services.Configure(options);
        services.AddMemoryCache();
        services.TryAddSingleton<IPermissionValidator, RemotePermissionValidator>();
        services.TryAddSingleton<IPermissionDefinitionManager, RemotePermissionDefinitionManager>();
        services.AddHttpClient(Constants.HttpClientName, (serviceProvider, httpClient) =>
        {
            var validationOptions = serviceProvider.GetRequiredService<IOptions<PermissionValidationOptions>>();
            httpClient.BaseAddress = new Uri(validationOptions.Value.Authority);
        });
        return services;
    }
}
