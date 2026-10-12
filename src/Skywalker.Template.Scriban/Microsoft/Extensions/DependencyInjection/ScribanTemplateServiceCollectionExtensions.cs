using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
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

        // SkywalkerTemplateRenderer 按 options.RenderingEngines 登记的具体类型从 DI 解析（#310）；
        // 未注册 IStringLocalizerFactory 时以 NullStringLocalizerFactory 兜底（#312）
        services.TryAddTransient(sp => ActivatorUtilities.CreateInstance<ScribanTemplateRenderingEngine>(
            sp, sp.GetService<IStringLocalizerFactory>() ?? NullStringLocalizerFactory.Instance));
        services.AddTransient<ITemplateRenderingEngine>(sp => sp.GetRequiredService<ScribanTemplateRenderingEngine>());

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

