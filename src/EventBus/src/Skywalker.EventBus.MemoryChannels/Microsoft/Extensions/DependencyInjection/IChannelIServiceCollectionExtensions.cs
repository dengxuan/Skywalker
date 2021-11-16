using Skywalker.EventBus.Abstractions;
using Skywalker.EventBus.Channels;

namespace Microsoft.Extensions.DependencyInjection;

public static class IChannelIServiceCollectionExtensions
{
    public static IServiceCollection AddChannelEventBus(this IServiceCollection services)
    {
        services.AddChannelsMessaging();
        services.AddSingleton<IEventBus, ChannelEventBus>();
        return services;
    }
}
