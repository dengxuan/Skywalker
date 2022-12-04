// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Template;
using Skywalker.Template.Abstractions;
using Skywalker.Template.VirtualFiles;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class TemplateIServiceCollectionExtensions
{
    public static IServiceCollection AddTemplate(this IServiceCollection services, Action<SkywalkerTextTemplatingOptions> options)
    {
        services.Configure(options);
        services.TryAddTransient<ITemplateContentContributor, VirtualFileTemplateContentContributor>();
        services.TryAddTransient<ITemplateRenderer, SkywallkerTemplateRenderer>();
        services.TryAddTransient<ITemplateContentProvider, TemplateContentProvider>();
        services.TryAddSingleton<ILocalizedTemplateContentReaderFactory, LocalizedTemplateContentReaderFactory>();
        services.TryAddSingleton<ITemplateDefinitionManager, TemplateDefinitionManager>();
        return services;
    }
}
