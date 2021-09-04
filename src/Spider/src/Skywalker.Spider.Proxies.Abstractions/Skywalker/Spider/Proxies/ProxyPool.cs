using Microsoft.Extensions.Logging;
using Skywalker.Extensions.WheelTimer;
using Skywalker.Spider.Proxies.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Skywalker.Spider.Proxies;

public class ProxyPool : IProxyPool
{

    private readonly ConcurrentQueue<ProxyEntry> _queue;
    private readonly ConcurrentDictionary<Uri, ProxyEntry> _dict;
    private readonly IProxyValidator _proxyValidator;
    private readonly ILogger<ProxyPool> _logger;

    private readonly HashedWheelTimer _timer = new(TimeSpan.FromSeconds(1), 100000);

    private readonly int _ignoreCount;
    private readonly int _reDetectCount;

    public ProxyPool(IProxyValidator proxyValidator, ILogger<ProxyPool> logger)
    {
        _proxyValidator = proxyValidator;
        _logger = logger;
        _queue = new ConcurrentQueue<ProxyEntry>();
        _dict = new ConcurrentDictionary<Uri, ProxyEntry>();
        _ignoreCount = 6;
        _reDetectCount = _ignoreCount / 2;
    }

    public async Task BackAsync(Uri proxy, HttpStatusCode statusCode)
    {
        if (_dict.TryGetValue(proxy, out var p))
        {
            // 若是返回成功，则直接把失败次数至为 0
            if (statusCode.IsSuccessStatusCode())
            {
                p.FailureCount = 0;
                p.SuccessCount++;
            }
            else
            {
                p.FailureCount += 1;
            }

            // 若是失败次数大于 ignoreCount，则把此代理从缓存中删除，不再使用
            if (p.FailureCount > _ignoreCount)
            {
                _dict.TryRemove(p.Uri, out _);
                return;
            }

            // 若是失败次数为 reDetectCount 的倍数则尝试重新测试此代理是否正常，若是测试不成功，则把此代理从缓存中删除，不再使用
            if (p.FailureCount == 0 || p.FailureCount % _reDetectCount != 0)
            {
                _queue.Enqueue(p);
                return;
            }
            bool isAvaliable = await _proxyValidator.IsAvailable(p.Uri);

            if (!isAvaliable)
            {
                _dict.TryRemove(p.Uri, out _);
            }
        }
    }

    public async Task<int> SetAsync(IEnumerable<Uri> proxies)
    {
        var cnt = 0;
        foreach (var proxy in proxies)
        {
            var isAvailable = await _proxyValidator.IsAvailable(proxy);
            if (!isAvailable)
            {
                continue;
            }
            ProxyEntry entry = new(proxy, TimeSpan.FromSeconds(10000));
            if (_dict.TryAdd(proxy, entry))
            {
                _logger.LogInformation($"proxy {proxy} is available");
                _timer.NewTimeout(new RecycleProxyTask(this, proxy), entry.Limited);
                _queue.Enqueue(_dict[proxy]);
                cnt++;
            }
        }

        return cnt;
    }

    public async Task<Uri?> GetAsync(int seconds)
    {
        var waitCount = seconds * 10;
        for (var i = 0; i < waitCount; ++i)
        {
            var proxy = Get();
            if (proxy != null)
            {
                return proxy;
            }

            await Task.Delay(100);
        }

        return null;
    }

    public Uri? Get()
    {
        if (_queue.TryDequeue(out var proxy))
        {
            _timer.NewTimeout(new BackProxyTask(this, proxy.Uri), TimeSpan.FromSeconds(30));
            return proxy.Uri;
        }
        else
        {
            return null;
        }
    }

    public Task RecycleAsync(Uri proxy)
    {
        _dict.TryRemove(proxy, out ProxyEntry _);
        return Task.CompletedTask;
    }
}
