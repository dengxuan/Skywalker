using Skywalker.Extensions.Emailing.Template;
using Skywalker.Extensions.VirtualFileSystem;
using Skywalker.Template;


namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring email template services.
/// </summary>
public static class EmailTemplateServiceCollectionExtensions
{
    /// <summary>
    /// Adds email template services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure email template options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEmailTemplate(
        this IServiceCollection services,
        Action<EmailTemplateOptions>? configureOptions = null)
    {
        // Configure options
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }

        // Register email template sender via auto services
        SkywalkerExtensionsEmailingTemplateAutoServiceExtensions.AddAutoServices(services);

        // Register template definition provider
        services.Configure<SkywalkerTextTemplatingOptions>(options =>
        {
            options.DefinitionProviders.Add<EmailTemplateDefinitionProvider>();
        });

        // Register virtual file system for embedded templates
        services.Configure<SkywalkerVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<EmailTemplateDefinitionProvider>(
                baseNamespace: "Skywalker.Extensions.Emailing.Template");
        });

        return services;
    }

    /// <summary>
    /// Adds email template services with default layout.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure email template options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEmailTemplateWithLayout(
        this IServiceCollection services,
        Action<EmailTemplateOptions>? configureOptions = null)
    {
        return services.AddEmailTemplate(options =>
        {
            options.DefaultLayoutTemplate = EmailTemplateNames.Layout;
            configureOptions?.Invoke(options);
        });
    }
}

