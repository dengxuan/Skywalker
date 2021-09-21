using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.Extensions.RateLimiters;
using Skywalker.Messaging.Abstractions;
using Skywalker.Spider.Abstractions;
using Skywalker.Spider.Http;
using Skywalker.Spider.Scheduler.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Spider;

internal class DefaultSpider<TRequestSupplier, TResponseHandler> : ISpider where TRequestSupplier : class, IRequestSupplier where TResponseHandler : class, IResponseHandler
{

    private readonly ConcurrentDictionary<string, Request> _requests = new();

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

    private readonly TResponseHandler _responseHandler;

    private readonly IThrottleStrategy _strategy;

    public DefaultSpider(IScheduler scheduler, IMessagePublisher messagePublisher, IMessageSubscriber messageSubscriber, TRequestSupplier requestSupplier, TResponseHandler responseHandler, IOptions<SpiderOptions> options, ILogger<DefaultSpider<TRequestSupplier, TResponseHandler>> logger)
    {
        _scheduler = scheduler;
        _messagePublisher = messagePublisher;
        _messageSubscriber = messageSubscriber;
        _requestSupplier = requestSupplier;
        _responseHandler = responseHandler;
        _logger = logger;
        _options = options.Value;
        _spiderId = new SpiderId(ObjectId.CreateId().ToString().ToUpper(), _options.SpiderName);
        _strategy = CreateBucket(_options.Speed);
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
        //else
        //{
        //    _logger.LogWarning($"{_options.SpiderName} enqueue request: {request.RequestUri}, {request.Hash} failed");
        //}
    }

    protected async Task HandleAsync(string routingKey, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken)
    {
        Response? response = await bytes.FromBytesAsync<Response>(cancellationToken);
        if (_requests.TryRemove(response!.RequestHash, out var request))
        {

            try
            {
                await _responseHandler.HandleAsync(request, response, cancellationToken);
                await _scheduler.SuccessAsync(request);
            }
            catch (Exception ex)
            {
                await _scheduler.FailAsync(request);
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
        _logger.LogInformation($"Initialize spider {_options.SpiderName}-{_spiderId.Id}");
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
                var isEmpty = await _scheduler.IsEmpty();
                if (isEmpty)
                {
                    await _scheduler.ResetDuplicateCheckAsync();
                    var requests = await _requestSupplier.GetAllListAsync(stoppingToken);
                    await _scheduler.EnqueueAsync(requests);
                }
                while (_requests.Count > _options.RequestedQueueCount)
                {
                    await Task.Delay(10, stoppingToken);
                }

                while (_strategy.ShouldThrottle(1, out var waitTimeMillis))
                {
                    await Task.Delay(waitTimeMillis, stoppingToken);
                }

                var request = await _scheduler.DequeueAsync();
                if (request != null)
                {
                    await PublishRequestMessagesAsync(request);
                    continue;
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
