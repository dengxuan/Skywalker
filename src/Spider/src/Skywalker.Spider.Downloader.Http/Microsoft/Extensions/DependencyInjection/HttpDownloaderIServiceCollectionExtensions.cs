using Skywalker.EventBus.Abstractions;
using Skywalker.Spider.Downloader.Abstractions;
using Skywalker.Spider.Http;
using Skywalker.Spider.HttpDownloader;

namespace Microsoft.Extensions.DependencyInjection;

public static class HttpDownloaderIServiceCollectionExtensions
{
    public static IServiceCollection AddHttpDownloader(this IServiceCollection services)
    {
        services.AddSingleton<IDownloader, HttpClientDownloader>();
        services.AddSingleton<IEventHandler<Request>, HttpRequestHandler>();
        services.AddHostedService<HttpDownloaderBackgroundService>();
        return services;
    }
}
