using Skywalker.Template;
using Skywalker.Template.Razor;

[assembly: Skywalker.SkywalkerModule("RazorTemplate")]

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding Razor template services.
/// </summary>
public static class RazorTemplateServiceCollectionExtensions
{
    /// <summary>
    /// Adds Razor template rendering engine to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Optional action to configure Razor options.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddRazorTemplate(
        this IServiceCollection services,
        Action<RazorTemplateOptions>? configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }

        SkywalkerTemplateRazorAutoServiceExtensions.AddAutoServices(services);

        // Register the engine in template options
        services.Configure<SkywalkerTextTemplatingOptions>(options =>
        {
            options.RenderingEngines[RazorTemplateRenderingEngine.EngineName] = typeof(RazorTemplateRenderingEngine);
        });

        return services;
    }
}

