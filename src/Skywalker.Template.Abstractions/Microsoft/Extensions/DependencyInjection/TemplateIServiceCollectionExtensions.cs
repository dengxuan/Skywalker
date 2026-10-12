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
    /// <summary>
    /// Adds Template services with default configuration.
    /// </summary>
    public static IServiceCollection AddTemplate(this IServiceCollection services)
    {
        // 默认内容贡献者经 ILocalizedTemplateContentReaderFactory 依赖 IVirtualFileProvider
        services.AddVirtualFileSystem();
        services.TryAddTransient<ITemplateContentContributor, VirtualFileTemplateContentContributor>();
        // TemplateContentProvider 按 options.ContentContributors 中的具体类型从 DI 解析，
        // 具体类型注册与默认登记缺一不可（#310）
        services.TryAddTransient<VirtualFileTemplateContentContributor>();
        services.Configure<SkywalkerTextTemplatingOptions>(options =>
        {
            if (!options.ContentContributors.Contains<VirtualFileTemplateContentContributor>())
            {
                options.ContentContributors.Add<VirtualFileTemplateContentContributor>();
            }
        });
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
        // 先登记默认项再执行调用方配置：ContentContributors 倒序查询，调用方追加的贡献者应优先于默认 VFS 贡献者
        services.AddTemplate();
        services.Configure(options);
        return services;
    }
}
