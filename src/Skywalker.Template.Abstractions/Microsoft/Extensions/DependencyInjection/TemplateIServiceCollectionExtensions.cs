// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Template;
using Skywalker.Template.Abstractions;
using Skywalker.Template.VirtualFiles;

[assembly: Skywalker.SkywalkerModule("Template")]

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class TemplateIServiceCollectionExtensions
{
    /// <summary>
    /// Adds Template services with default configuration.
    /// </summary>
    public static IServiceCollection AddTemplate(this IServiceCollection services)
    {
        services.TryAddTransient<ITemplateContentContributor, VirtualFileTemplateContentContributor>();
        services.TryAddTransient<ITemplateRenderer, SkywalkerTemplateRenderer>();
        services.TryAddTransient<ITemplateContentProvider, TemplateContentProvider>();
        services.TryAddSingleton<ILocalizedTemplateContentReaderFactory, LocalizedTemplateContentReaderFactory>();
        services.TryAddSingleton<ITemplateDefinitionManager, TemplateDefinitionManager>();
        return services;
    }

    /// <summary>
    /// Adds Template services with configuration.
    /// </summary>
    public static IServiceCollection AddTemplate(this IServiceCollection services, Action<SkywalkerTextTemplatingOptions> options)
    {
        services.Configure(options);
        return services.AddTemplate();
    }
}
