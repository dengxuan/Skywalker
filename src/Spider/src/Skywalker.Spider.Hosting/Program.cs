// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skywalker.Spider.Abstractions;
using Skywalker.Spider.Hosting;

await Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
{
    services.AddSpider();
    services.AddChannelEventBus();
    services.AddHttpDownloader();
    services.AddBeikeProxy();
    services.AddSingleton<IRequestSupplier, RequestSupplier>();
})
.RunConsoleAsync();
