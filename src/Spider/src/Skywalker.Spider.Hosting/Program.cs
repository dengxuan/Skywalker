using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TiTools.Spider.Hosting;

await Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
{
    services.AddSpider(builder =>
    {
        builder.UseSpider<AvailableRequestSupplier, AvailableResponseHandler>();
        builder.UseSpider<InventoryRequestSupplier, InventoryResponseHandler>();
        builder.UseSpider<GenericPartNumberRequestSupplier, GenericPartNumberResponseHandler>();
    });
    services.AddChannelsMessaging();
    services.AddHttpDownloader();
    services.AddBeikeProxy();
    services.AddLogging(builder =>
    {
        builder.AddFile();
    });
})
.RunConsoleAsync();