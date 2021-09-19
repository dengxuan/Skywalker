using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skywalker.Spider.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Spider;

internal class HostedSpiderService : BackgroundService
{
    private readonly IEnumerable<ISpider<IRequestSupplier>> _spiders;

    public HostedSpiderService(IServiceProvider serviceProvider)
    {
        _spiders = serviceProvider.GetRequiredService<ISpiderBuilder>().CreateSpider(serviceProvider);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tasks = new List<Task>();
        foreach (var spider in _spiders)
        {
            Task spiderTasker = Task.Factory.StartNew(async () =>
            {
                await spider.InitializeAsync(stoppingToken);
                await spider.ExecuteAsync(stoppingToken);
                await spider.UnInitializeAsync(stoppingToken);
            }, TaskCreationOptions.LongRunning);
            tasks.Add(spiderTasker);
        }
        return Task.WhenAll(tasks);
    }
}
