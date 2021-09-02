// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skywalker.EventBus;
using Skywalker.EventBus.Abstractions;
using Skywalker.Spider.Http;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddLogging();
        services.AddSpider();
        services.AddMemoryChannels();
        services.AddHttpDownloader();
    })
    .Build();
IEventBus eventBus = host.Services.GetRequiredService<IEventBus>();
IServiceScopeFactory serviceScopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();
eventBus.Subscribe<Request>(new IocEventHandlerFactory(serviceScopeFactory, typeof(IEventHandler<Request>)));
eventBus.Subscribe<Response>(new IocEventHandlerFactory(serviceScopeFactory, typeof(IEventHandler<Response>)));

await host.RunAsync();
