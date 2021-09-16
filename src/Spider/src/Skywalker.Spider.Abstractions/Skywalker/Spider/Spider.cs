using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.EventBus;
using Skywalker.EventBus.Abstractions;
using Skywalker.Extensions.RateLimiters;
using Skywalker.Spider.Abstractions;
using Skywalker.Spider.Http;
using Skywalker.Spider.Scheduler.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Spider
{
    internal class Spider : BackgroundService
    {

        private readonly IEventBus _eventBus;

        private readonly IScheduler _scheduler;

        private readonly InProgressRequests _inProgressRequests;

        private readonly IList<IRequestSupplier> _requestSuppliers;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        protected SpiderOptions Options { get; }

        /// <summary>
        /// 日志接口
        /// </summary>
        protected ILogger Logger { get; }

        public Spider(IEventBus eventBus, IScheduler scheduler, InProgressRequests inProgressRequests, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, IOptions<SpiderOptions> options, ILogger<Spider> logger)
        {
            _eventBus = eventBus;
            _scheduler = scheduler;
            _requestSuppliers = serviceProvider.GetServices<IRequestSupplier>().ToList();
            _inProgressRequests = inProgressRequests;
            _serviceScopeFactory = serviceScopeFactory;
            Logger = logger;
            Options = options.Value;
        }

        private static FixedTokenBucket CreateBucket(double speed)
        {
            if (speed >= 1)
            {
                var defaultTimeUnit = (int)(1000 / speed);
                return new FixedTokenBucket(1, 1, defaultTimeUnit);
            }
            else
            {
                var defaultTimeUnit = (int)(1 / speed * 1000);
                return new FixedTokenBucket(1, 1, defaultTimeUnit);
            }
        }

        private async Task PublishRequestMessagesAsync(Request request)
        {

            //string topic = request.Policy switch
            //{
            //    RequestPolicy.Random => $"{request.Agent}",
            //    RequestPolicy.Chained => request.Agent ?? request.Downloader ?? DownloaderTypes.Http.ToString(),
            //    _ => DownloaderTypes.Http.ToString(),
            //};

            if (_inProgressRequests.Enqueue(request))
            {
                request.Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                await _eventBus.PublishAsync(request);
            }
            else
            {
                Logger.LogWarning($"{Options.SpiderId} enqueue request: {request.RequestUri}, {request.Hash} failed");
            }
        }

        /// <summary>
        /// 初始化爬虫数据
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected async Task InitializeAsync(CancellationToken stoppingToken = default)
        {
            Logger.LogInformation($"Initialize spider {Options.SpiderId}");

            _eventBus.Subscribe<Response>(new IocEventHandlerFactory(_serviceScopeFactory, typeof(IEventHandler<Response>)));

            // 通过供应接口添加请求
            foreach (var requestSupplier in _requestSuppliers)
            {
                var requests = await requestSupplier.GetAllListAsync(stoppingToken);
                await _scheduler.EnqueueAsync(requests);

                Logger.LogInformation($"{Options.SpiderId} load request from {requestSupplier.GetType().Name} {_requestSuppliers.IndexOf(requestSupplier)}/{_requestSuppliers.Count}");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var bucket = CreateBucket(Options.Speed);

                    while (_inProgressRequests.Count > Options.RequestedQueueCount)
                    {
                        await Task.Delay(10, stoppingToken);
                    }

                    while (bucket.ShouldThrottle(1, out var waitTimeMillis))
                    {
                        await Task.Delay(waitTimeMillis, stoppingToken);
                    }

                    var requests = await _scheduler.DequeueAsync(Options.Batch);

                    foreach (var request in requests)
                    {
                        await PublishRequestMessagesAsync(request);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e, $"{Options.SpiderId}");
                }
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await InitializeAsync(cancellationToken);
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _eventBus.Unsubscribe<Response>(new IocEventHandlerFactory(_serviceScopeFactory, typeof(IEventHandler<Response>)));

            await base.StopAsync(cancellationToken);

            Logger.LogInformation($"{Options.SpiderId} stopped");
        }
    }
}
