using Skywalker.Spider.Abstractions;
using Skywalker.Spider.Messaging;
using Skywalker.Spider.Messaging.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class SpiderMessagingIServiceCollectionExtensions
{
    public static ISpiderBuilder AddMessaging(this ISpiderBuilder spiderBuilder)
    {
        spiderBuilder.Services.AddSingleton<IMessager, DefaultMessager>();
        return spiderBuilder;
    }
}
