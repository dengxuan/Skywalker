using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.Extensions.RateLimiters;
using Skywalker.Messaging.Abstractions;
using Skywalker.Spider.Abstractions;
using Skywalker.Spider.Http;
using Skywalker.Spider.Pipelines;
using Skywalker.Spider.Pipelines.Abstractions;
using Skywalker.Spider.Scheduler.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Spider
{
    internal class DefaultSpider<TRequestSupplier> : ISpider<TRequestSupplier> where TRequestSupplier : IRequestSupplier
    {

        /// <summary>
        /// 日志接口
        /// </summary>
        private readonly ILogger _logger;

        private readonly SpiderId _spiderId;

        private readonly SpiderOptions _options;

        private readonly IScheduler _scheduler;

        private readonly IMessagePublisher _messagePublisher;

        private readonly IMessageSubscriber _messageSubscriber;

        private readonly TRequestSupplier _requestSupplier;

        private readonly PipelineDelegate _pipeline;

        private readonly ConcurrentDictionary<string, Request> _requests = new();

        public DefaultSpider(IScheduler scheduler, IMessagePublisher messagePublisher, IMessageSubscriber messageSubscriber, TRequestSupplier requestSupplier, IPipelineChainBuilder pipelineChainBuilder, IOptions<SpiderOptions> options, ILogger<DefaultSpider<TRequestSupplier>> logger)
        {
            _scheduler = scheduler;
            _messagePublisher = messagePublisher;
            _messageSubscriber = messageSubscriber;
            _requestSupplier = requestSupplier;
            _logger = logger;
            _options = options.Value;
            _pipeline = pipelineChainBuilder.Build();
            _spiderId = new SpiderId(ObjectId.CreateId().ToString(), _options.SpiderName);
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
            request.Owner = _spiderId.Id;
            if (_requests.TryAdd(request.Hash, request))
            {
                request.Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                byte[] bytes = await request.ToBytesAsync();
                await _messagePublisher.PublishAsync(request.Downloader, bytes);
            }
            else
            {
                _logger.LogWarning($"{_options.SpiderName} enqueue request: {request.RequestUri}, {request.Hash} failed");
            }
        }

        protected async Task HandleAsync(string routingKey, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken)
        {
            Response? response = await bytes.FromBytesAsync<Response>(cancellationToken);
            if (_requests.TryRemove(response!.RequestHash, out var request))
            {

                try
                {
                    using var context = new PipelineContext(request, response);
                    await _pipeline(context);
                    var count = await _scheduler.EnqueueAsync(context.FollowRequests);
                    await _scheduler.SuccessAsync(request!);
                }
                catch (Exception ex)
                {
                    await _scheduler.FailAsync(request!);
                    _logger.LogError(ex, "Handle {0} failed: {1}", JsonSerializer.Serialize(request), ex.Message);
                }
            }
        }

        /// <summary>
        /// 初始化爬虫数据
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        public async Task InitializeAsync(CancellationToken stoppingToken = default)
        {
            _logger.LogInformation($"Initialize spider {_options.SpiderName}");
            await _messageSubscriber.SubscribeAsync(_spiderId.Id, HandleAsync, stoppingToken);

            // 通过供应接口添加请求
            var requests = await _requestSupplier.GetAllListAsync(stoppingToken);
            await _scheduler.EnqueueAsync(requests);

            _logger.LogInformation($"{_options.SpiderName} load request from {_requestSupplier.GetType().Name}");
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var bucket = CreateBucket(_options.Speed);

                    while (_requests.Count > _options.RequestedQueueCount)
                    {
                        await Task.Delay(10, stoppingToken);
                    }

                    while (bucket.ShouldThrottle(1, out var waitTimeMillis))
                    {
                        await Task.Delay(waitTimeMillis, stoppingToken);
                    }

                    var requests = await _scheduler.DequeueAsync(_options.Batch);

                    foreach (var request in requests)
                    {
                        await PublishRequestMessagesAsync(request);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"{_options.SpiderName}");
                }
            }
        }

        public async Task UnInitializeAsync(CancellationToken cancellationToken)
        {
            await _messageSubscriber.UnsubscribeAsync(_spiderId.Id, cancellationToken);
            _logger.LogInformation($"{_options.SpiderName} stopped");
        }
    }
}
