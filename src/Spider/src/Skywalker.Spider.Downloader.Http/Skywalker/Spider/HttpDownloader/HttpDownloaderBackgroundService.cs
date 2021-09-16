using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skywalker.EventBus;
using Skywalker.EventBus.Abstractions;
using Skywalker.Spider.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Spider.HttpDownloader
{
    internal class HttpDownloaderBackgroundService : BackgroundService
    {
        private readonly IEventBus _eventBus;

        private readonly IServiceScopeFactory _serviceProviderFactory;
        public HttpDownloaderBackgroundService(IEventBus eventBus, IServiceScopeFactory serviceScopeFactory)
        {
            _eventBus = eventBus;
            _serviceProviderFactory = serviceScopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _eventBus.Subscribe<Request>(new IocEventHandlerFactory(_serviceProviderFactory, typeof(IEventHandler<Request>)));
            return Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
