// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Skywalker;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Skywalker 构建器的 Entity Framework Core 扩展方法。
/// </summary>
public static class SkywalkerBuilderEntityFrameworkCoreExtensions
{
    /// <summary>
    /// 添加 Entity Framework Core 支持。
    /// </summary>
    /// <typeparam name="TDbContext">DbContext 类型。</typeparam>
    /// <param name="builder">Skywalker 构建器。</param>
    /// <param name="optionsAction">DbContext 配置选项。</param>
    /// <returns>Skywalker 构建器，支持链式调用。</returns>
    /// <remarks>
    /// 使用示例：
    /// <code>
    /// services.AddSkywalker()
    ///     .AddEntityFramework&lt;MyDbContext&gt;(options =>
    ///         options.UseSqlServer(connectionString));
    /// </code>
    /// </remarks>
    public static ISkywalkerBuilder AddEntityFramework<TDbContext>(
        this ISkywalkerBuilder builder,
        Action<DbContextOptionsBuilder<TDbContext>> optionsAction)
        where TDbContext : SkywalkerDbContext<TDbContext>
    {
        builder.Services.AddSkywalkerDbContext<TDbContext>(skywalkerOptions =>
        {
            skywalkerOptions.Configure<TDbContext>(context => optionsAction(context.DbContextOptions));
        });
        return builder;
    }

    /// <summary>
    /// 添加 Entity Framework Core 支持（高级配置）。
    /// </summary>
    /// <typeparam name="TDbContext">DbContext 类型。</typeparam>
    /// <param name="builder">Skywalker 构建器。</param>
    /// <param name="configureAction">高级配置选项，可访问连接字符串、服务提供者等。</param>
    /// <returns>Skywalker 构建器，支持链式调用。</returns>
    /// <remarks>
    /// 使用示例：
    /// <code>
    /// services.AddSkywalker()
    ///     .AddEntityFramework&lt;MyDbContext&gt;(context =>
    ///         context.DbContextOptions.UseSqlServer(context.ConnectionString));
    /// </code>
    /// </remarks>
    public static ISkywalkerBuilder AddEntityFramework<TDbContext>(
        this ISkywalkerBuilder builder,
        Action<SkywalkerDbContextConfigurationContext<TDbContext>> configureAction)
        where TDbContext : SkywalkerDbContext<TDbContext>
    {
        builder.Services.AddSkywalkerDbContext<TDbContext>(skywalkerOptions =>
        {
            skywalkerOptions.Configure(configureAction);
        });
        return builder;
    }
}
