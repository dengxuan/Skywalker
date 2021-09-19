// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skywalker.Spider.Hosting;
using Skywalker.Spider.Pipelines.Extensions;


await Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
{
    services.AddSpider(builder =>
    {
        builder.UseSpider<RequestSupplier>(pipeline =>
        {
            pipeline.UseMiddleware<Middleware>();
            pipeline.Use((context, next) =>
            {
                System.Console.WriteLine("Inline");
                return next(context);
            });
        });
    });
    services.AddChannelEventBus();
    services.AddHttpDownloader();
    services.AddBeikeProxy();
})
.RunConsoleAsync();
