// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.GuidGenerator;

namespace Microsoft.Extensions.DependencyInjection;

public static class GuidGeneratorIServiceCollectionExtensions
{
    internal static IServiceCollection AddGuidGeneratorServices(this IServiceCollection services)
    {
        services.AddSingleton<IGuidGenerator, SequentialGuidGenerator>();
        return services;
    }

    public static IServiceCollection AddGuidGenerator(this IServiceCollection services)
    {
        return services.AddGuidGeneratorServices();
    }

    public static IServiceCollection AddGuidGenerator(this IServiceCollection service, Action<SequentialGuidGeneratorOptions> options)
    {
        service.Configure(options);
        return service.AddGuidGeneratorServices();
    }
}
