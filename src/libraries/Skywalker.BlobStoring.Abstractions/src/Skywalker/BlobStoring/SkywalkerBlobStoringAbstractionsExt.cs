// Licensed to the zshiot.com under one or more agreements.
// zshiot.com licenses this file to you under the license.

using Microsoft.Extensions.DependencyInjection;
using Skywalker.BlobStoring.Abstractions;

namespace Skywalker.BlobStoring;
public static class SkywalkerBlobStoringAbstractionsExt
{
    public static IServiceCollection AddBlobStoring(this IServiceCollection services,
        Action<AbpBlobStoringOptions> options)
    {
        services.Configure(options);

        services.AddTransient(
            typeof(IBlobContainer<>),
            typeof(BlobContainer<>)
        );

        services.AddTransient(
            typeof(IBlobContainer),
            serviceProvider => serviceProvider
                .GetRequiredService<IBlobContainer<DefaultContainer>>()
        );

        return services;
    }
}
