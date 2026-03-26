// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Microsoft.Extensions.DependencyInjection;

public static class JsonSerializerIServiceCollectionExtensions
{
    public static IServiceCollection AddJsonSerializer(this IServiceCollection services)
    {
        services.AddTimezone();
        SkywalkerSerializationSystemTextJsonAutoServiceExtensions.AddAutoServices(services);
        return services;
    }
}
