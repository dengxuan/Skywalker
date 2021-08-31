using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.EventBus.Abstractions;
using Skywalker.Extensions.RateLimiters;
using Skywalker.Spider.Downloader;
using Skywalker.Spider.Http;
using Skywalker.Spider.Scheduler.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Spider.Abstractions
{
    public class SpiderBackgroundService : BackgroundService
    {
        private readonly IList<IRequestSupplier> _requestSuppliers;
        private readonly InProgressRequests _inProgressRequests;
        private readonly string _defaultDownloader;
        private readonly IEventBus _eventBus;
        private readonly IScheduler _scheduler;

        /// <summary>
        /// 请求 Timeout 事件
        /// </summary>
        protected event Action<Request[]>? OnRequestTimeout;

        protected SpiderOptions Options { get; }

        /// <summary>
        /// 日志接口
        /// </summary>
        protected ILogger Logger { get; }

        public SpiderBackgroundService(IOptions<SpiderOptions> options,
            IEventBus eventBus,
            IScheduler scheduler,
            IServiceProvider services,
            InProgressRequests inProgressRequests,
            HostBuilderContext hostBuilderContext,
            ILogger<SpiderBackgroundService> logger
        )
        {
            Logger = logger;
            Options = options.Value;

            if (Options.Speed > 500)
            {
                throw new SpiderException("Speed should not large than 500");
            }

            _eventBus = eventBus;
            _scheduler = scheduler;
            _inProgressRequests = inProgressRequests;
            _requestSuppliers = services.GetServices<IRequestSupplier>().ToList();
            _defaultDownloader = hostBuilderContext.Properties.ContainsKey(Constants.DefaultDownloader)
                ? hostBuilderContext.Properties[Constants.DefaultDownloader]?.ToString()!
                : DownloaderTypes.Http.ToString();
        }

        /// <summary>
        /// 初始化爬虫数据
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected async Task InitializeAsync(CancellationToken stoppingToken = default)
        {
            //SpiderId = GenerateSpiderId();
            Logger.LogInformation($"Initialize spider {Options.SpiderId}");
            await _scheduler.InitializeAsync(Options.SpiderId);
            await LoadRequestFromSuppliers(stoppingToken);
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await InitializeAsync(cancellationToken);
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            //Todo Unsubscribe

            await base.StopAsync(cancellationToken);

            Dispose();

            Logger.LogInformation($"{Options.SpiderId} stopped");
        }

        protected async Task<int> AddRequestsAsync(IEnumerable<Request> requests)
        {
            if (requests.IsNullOrEmpty())
            {
                return 0;
            }

            var list = new List<Request>();

            foreach (var request in requests)
            {
                if (string.IsNullOrWhiteSpace(request.Downloader)
                    && !string.IsNullOrWhiteSpace(_defaultDownloader))
                {
                    request.Downloader = _defaultDownloader;
                }

                request.RequestedTimes += 1;

                // 1. 请求次数超过限制则跳过，并添加失败记录
                // 2. 默认构造的请求次数为 0， 并且不允许用户更改，因此可以保证数据安全性
                if (request.RequestedTimes > Options.RetriedTimes)
                {
                    //Todo: Statistics
                    //await _services.StatisticsClient.IncreaseFailureAsync(SpiderId.Id);
                    continue;
                }

                // 1. 默认构造的深度为 0， 并且不允许用户更改，因此可以保证数据安全性
                // 2. 当深度超过限制则跳过
                if (Options.Depth > 0 && request.Depth > Options.Depth)
                {
                    continue;
                }

                request.Owner = Options.SpiderId;

                list.Add(request);
            }

            var count = await _scheduler.EnqueueAsync(list);
            return count;
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

        private async Task<bool> HandleTimeoutRequestAsync()
        {
            var timeoutRequests = _inProgressRequests.GetAllTimeoutList();
            if (timeoutRequests.Length <= 0)
            {
                return false;
            }

            foreach (var request in timeoutRequests)
            {
                Logger.LogWarning(
                    $"{Options.SpiderId} request {request.RequestUri}, {request.Hash} timeout");
            }

            await AddRequestsAsync(timeoutRequests);

            OnRequestTimeout?.Invoke(timeoutRequests);

            return true;
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

        protected async Task LoadRequestFromSuppliers(CancellationToken stoppingToken)
        {
            await AddRequestsAsync(new Request[] { new Request("https://www.baidu.com") });
            // 通过供应接口添加请求
            foreach (var requestSupplier in _requestSuppliers)
            {
                var requests = await requestSupplier.GetAllListAsync(stoppingToken);
                await _scheduler.EnqueueAsync(requests);

                Logger.LogInformation($"{Options.SpiderId} load request from {requestSupplier.GetType().Name} {_requestSuppliers.IndexOf(requestSupplier)}/{_requestSuppliers.Count}");
            }
        }

        public override void Dispose()
        {
            _inProgressRequests?.Dispose();

            base.Dispose();

            GC.Collect();
        }
    }
}
