using Skywalker.Spider.Downloader.Abstractions;
using Skywalker.Spider.HttpDownloader;

namespace Microsoft.Extensions.DependencyInjection;

public static class HttpDownloaderIServiceCollectionExtensions
{
    public static IServiceCollection AddHttpDownloader(this IServiceCollection services)
    {
        services.AddSingleton<IDownloader, HttpClientDownloader>();
        services.AddHostedService<HostedHttpDownloader>();
        return services;
    }
}
