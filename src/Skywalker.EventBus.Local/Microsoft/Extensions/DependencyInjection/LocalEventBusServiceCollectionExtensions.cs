using Microsoft.Extensions.DependencyInjection;
using Skywalker.EventBus;
using Skywalker.EventBus.Abstractions;
using Skywalker.EventBus.Local;


namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding local channel event bus services.
/// </summary>
public static class LocalEventBusServiceCollectionExtensions
{
    /// <summary>
    /// Adds the local channel event bus with configuration from appsettings.json.
    /// Configuration section: Skywalker:EventBus:Local
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddEventBusLocal(this IServiceCollection services)
    {
        // 添加 EventBus 核心服务
        services.AddEventBusCore();

        // 从配置文件绑定选项
        var configuration = services.GetConfiguration();
        if (configuration != null)
        {
            services.AddOptions<LocalEventBusOptions>()
                .Bind(configuration.GetSection(LocalEventBusOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }
        else
        {
            // 没有配置时使用默认�?
            services.AddOptions<LocalEventBusOptions>()
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }

        return services.AddEventBusLocalServices();
    }

    /// <summary>
    /// Adds the local channel event bus with configuration action.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddEventBusLocal(this IServiceCollection services, Action<LocalEventBusOptions> configure)
    {
        // 添加 EventBus 核心服务
        services.AddEventBusCore();

        // 先从配置文件绑定，再应用代码配置
        var configuration = services.GetConfiguration();
        if (configuration != null)
        {
            services.AddOptions<LocalEventBusOptions>()
                .Bind(configuration.GetSection(LocalEventBusOptions.SectionName))
                .Configure(configure)
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }
        else
        {
            services.AddOptions<LocalEventBusOptions>()
                .Configure(configure)
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }

        return services.AddEventBusLocalServices();
    }

    private static IServiceCollection AddEventBusLocalServices(this IServiceCollection services)
    {
        services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddSingleton<ILocalEventBus, LocalChannelEventBus>();
        // 注册 IEventBus 指向 ILocalEventBus 的实�?
        services.AddSingleton<IEventBus>(sp => sp.GetRequiredService<ILocalEventBus>());

        return services;
    }
}

