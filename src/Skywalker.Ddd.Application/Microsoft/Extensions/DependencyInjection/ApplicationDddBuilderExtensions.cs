// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker;
using Skywalker.ApplicationParts;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// DDD Application 模块扩展方法。
/// </summary>
public static class ApplicationDddBuilderExtensions
{
    /// <summary>
    /// 添加 DDD Application 模块，自动发现并注册所有 <see cref="Skywalker.Ddd.Application.Abstractions.IApplicationService"/> 实现。
    /// </summary>
    /// <param name="builder">Skywalker 构建器。</param>
    /// <returns>Skywalker 构建器，支持链式调用。</returns>
    public static ISkywalkerBuilder AddDddApplication(this ISkywalkerBuilder builder)
    {
        var provider = new ApplicationServiceFeatureProvider();
        builder.PartManager.FeatureProviders.Add(provider);

        var feature = new ServiceRegistrationFeature();
        provider.PopulateFeature(builder.PartManager.ApplicationParts, feature);

        foreach (var descriptor in feature.Services)
        {
            builder.Services.TryAdd(descriptor);
        }

        builder.Services.AddInterceptedServices();

        return builder;
    }
}
