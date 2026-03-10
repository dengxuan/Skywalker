using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Skywalker.Localization.AspNetCore;

/// <summary>
/// Extension methods for adding localization services.
/// </summary>
public static class LocalizationServiceCollectionExtensions
{
    /// <summary>
    /// Adds Skywalker localization services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure localization options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSkywalkerLocalization(
        this IServiceCollection services,
        Action<LocalizationOptions>? configureOptions = null)
    {
        services.Configure<LocalizationOptions>(options =>
        {
            configureOptions?.Invoke(options);
        });

        services.TryAddSingleton<IStringLocalizerFactory, SkywalkerStringLocalizerFactory>();
        services.TryAddTransient(typeof(IStringLocalizer<>), typeof(SkywalkerStringLocalizer<>));
        services.TryAddSingleton<ILanguageProvider, DefaultLanguageProvider>();

        return services;
    }

    /// <summary>
    /// Configures request localization based on <see cref="LocalizationOptions"/>.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseSkywalkerRequestLocalization(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<Microsoft.Extensions.Options.IOptions<LocalizationOptions>>();
        var languages = options.Value.Languages;

        if (languages.Count == 0)
        {
            return app;
        }

        var supportedCultures = languages.Select(l => l.CultureName).ToArray();
        var defaultCulture = languages.FirstOrDefault(l => l.IsDefault)?.CultureName ?? supportedCultures[0];

        var requestLocalizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture(defaultCulture)
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);

        return app.UseRequestLocalization(requestLocalizationOptions);
    }
}

