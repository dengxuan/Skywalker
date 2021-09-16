using Skywalker.Messaging.Abstractions;
using Skywalker.Messaging.Channels;
using Skywalker.Messaging.Channels.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ChannelMessagingIServiceCollectionExtensions
    {
        public static IServiceCollection AddChannelsMessaging(this IServiceCollection services)
        {
            services.AddSingleton<ISubscriber, Subscriber>();
            services.AddSingleton<IMessagePublisher, ChannelMessagePublisher>();
            services.AddSingleton<IMessageSubscriber, ChannelMessageSubscriber>();
            return services;
        }
    }
}
