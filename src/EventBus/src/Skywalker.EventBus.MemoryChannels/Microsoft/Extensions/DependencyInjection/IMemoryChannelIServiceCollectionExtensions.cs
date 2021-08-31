using Skywalker.EventBus.Abstractions;
using Skywalker.EventBus.MemoryChannels;

namespace Microsoft.Extensions.DependencyInjection;

public static class IMemoryChannelIServiceCollectionExtensions
{
    public static IServiceCollection AddMemoryChannels(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionFactory, ConnectionFactoryFactory>();
        services.AddSingleton<IChannelMessageConsumer, ChannelMessageConsumer>();
        services.AddSingleton<IEventBus, MemoryChannelEventBus>();
        return services;
    }
}
