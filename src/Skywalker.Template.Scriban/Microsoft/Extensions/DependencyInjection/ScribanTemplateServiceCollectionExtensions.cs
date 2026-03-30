using Skywalker.Template;
using Skywalker.Template.Abstractions;
using Skywalker.Template.Scriban;


namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding Scriban template services.
/// </summary>
public static class ScribanTemplateServiceCollectionExtensions
{
    /// <summary>
    /// Adds Scriban template rendering engine to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Optional action to configure Scriban options.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddScribanTemplate(
        this IServiceCollection services,
        Action<ScribanTemplateOptions>? configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }

        services.AddTransient<ITemplateRenderingEngine, ScribanTemplateRenderingEngine>();

        // Register the engine in template options
        services.Configure<SkywalkerTextTemplatingOptions>(options =>
        {
            options.RenderingEngines[ScribanTemplateRenderingEngine.EngineName] = typeof(ScribanTemplateRenderingEngine);

            if (string.IsNullOrEmpty(options.DefaultRenderingEngine))
            {
                options.DefaultRenderingEngine = ScribanTemplateRenderingEngine.EngineName;
            }
        });

        return services;
    }
}

