// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker;
using Skywalker.EventBus.RabbitMQ;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Skywalker 构建器的 RabbitMQ 事件总线扩展方法。
/// </summary>
public static class SkywalkerBuilderRabbitMQEventBusExtensions
{
    /// <summary>
    /// 添加 RabbitMQ 事件总线支持，替换默认的本地事件总线。
    /// </summary>
    /// <param name="builder">Skywalker 构建器。</param>
    /// <returns>Skywalker 构建器，支持链式调用。</returns>
    /// <remarks>
    /// 配置从 appsettings.json 的 Skywalker:EventBus:RabbitMQ 节读取。
    /// <code>
    /// services.AddSkywalker()
    ///     .AddRabbitMQEventBus();
    /// </code>
    /// </remarks>
    public static ISkywalkerBuilder AddRabbitMQEventBus(this ISkywalkerBuilder builder)
    {
        builder.Services.AddEventBusRabbitMQ();
        return builder;
    }

    /// <summary>
    /// 添加 RabbitMQ 事件总线支持，替换默认的本地事件总线。
    /// </summary>
    /// <param name="builder">Skywalker 构建器。</param>
    /// <param name="configure">RabbitMQ 配置选项。</param>
    /// <returns>Skywalker 构建器，支持链式调用。</returns>
    /// <remarks>
    /// <code>
    /// services.AddSkywalker()
    ///     .AddRabbitMQEventBus(options =>
    ///     {
    ///         options.HostName = "localhost";
    ///         options.UserName = "guest";
    ///         options.Password = "guest";
    ///     });
    /// </code>
    /// </remarks>
    public static ISkywalkerBuilder AddRabbitMQEventBus(this ISkywalkerBuilder builder, Action<RabbitMqEventBusOptions> configure)
    {
        builder.Services.AddEventBusRabbitMQ(configure);
        return builder;
    }
}
