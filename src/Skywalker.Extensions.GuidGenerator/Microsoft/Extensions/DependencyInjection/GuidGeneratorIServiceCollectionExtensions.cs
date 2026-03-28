// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Extensions.GuidGenerator;


namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding GUID generator services.
/// </summary>
public static class GuidGeneratorIServiceCollectionExtensions
{
    /// <summary>
    /// Adds GUID generator services with configuration from appsettings.json.
    /// Configuration section: Skywalker:GuidGenerator
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddGuidGenerator(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();
        if (configuration != null)
        {
            services.AddOptions<SequentialGuidGeneratorOptions>()
                .Bind(configuration.GetSection(SequentialGuidGeneratorOptions.SectionName))
                .ValidateOnStart();
        }
        else
        {
            services.AddOptions<SequentialGuidGeneratorOptions>()
                .ValidateOnStart();
        }

        services.AddAutoServices();
        return services;
    }

    /// <summary>
    /// Adds GUID generator services with configuration action.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddGuidGenerator(this IServiceCollection services, Action<SequentialGuidGeneratorOptions> configure)
    {
        var configuration = services.GetConfiguration();
        if (configuration != null)
        {
            services.AddOptions<SequentialGuidGeneratorOptions>()
                .Bind(configuration.GetSection(SequentialGuidGeneratorOptions.SectionName))
                .Configure(configure)
                .ValidateOnStart();
        }
        else
        {
            services.AddOptions<SequentialGuidGeneratorOptions>()
                .Configure(configure)
                .ValidateOnStart();
        }

        services.AddAutoServices();
        return services;
    }
}
