using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

    private readonly ConcurrentQueue<ProxyEntry> _queue = new();
    private readonly ConcurrentDictionary<Uri, ProxyEntry> _proxies = new();
    private readonly IProxyValidator _proxyValidator;
    private readonly IProxyStorage _proxyStorage;
    private readonly ILogger<ProxyPool> _logger;
    private readonly ProxyOptions _options;

    private readonly HashedWheelTimer _timer = new(TimeSpan.FromSeconds(1), 100);

    public ProxyPool(IProxyValidator proxyValidator, IProxyStorage proxyStorage, IOptions<ProxyOptions> options, ILogger<ProxyPool> logger)
    {
        _proxyValidator = proxyValidator;
        _proxyStorage = proxyStorage;
        _logger = logger;
        _options = options.Value;
    }

    private bool TryAdd(ProxyEntry entry)
    {
        if (_proxies.TryAdd(entry.Uri, entry))
        {
            _logger.LogInformation($"proxy {entry.Uri} is available");
            _timer.NewTimeout(new RecycleProxyTask(this, entry.Uri), entry.ExpireTime - DateTime.Now);
            _queue.Enqueue(_proxies[entry.Uri]);
            return true;
        }
        return false;
    }

    public async Task InitializeAsync()
    {
        var entries = await _proxyStorage.GetAvailablesAsync();
        foreach (var entry in entries)
        {
            TryAdd(entry);
        }
    }

    public async Task BackAsync(Uri proxy, HttpStatusCode statusCode)
    {
        if (_proxies.TryGetValue(proxy, out var p))
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
            if (p.FailureCount > _options.IgnoreCount)
            {
                _proxies.TryRemove(p.Uri, out _);
                return;
            }

            // 若是失败次数为 reDetectCount 的倍数则尝试重新测试此代理是否正常，若是测试不成功，则把此代理从缓存中删除，不再使用
            if (p.FailureCount == 0 || p.FailureCount % _options.RedetectCount != 0)
            {
                _queue.Enqueue(p);
                return;
            }
            bool isAvaliable = await _proxyValidator.IsAvailable(p.Uri);

            if (!isAvaliable)
            {
                _proxies.TryRemove(p.Uri, out _);
            }
        }
    }

    public async Task<int> SetAsync(IEnumerable<ProxyEntry> proxies)
    {
        var cnt = 0;
        foreach (var entry in proxies)
        {
            var isAvailable = await _proxyValidator.IsAvailable(entry.Uri);
            if (!isAvailable)
            {
                continue;
            }
            await _proxyStorage.CreateAsync(entry);
            if (TryAdd(entry))
            {
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
        _proxies.TryRemove(proxy, out ProxyEntry _);
        _logger.LogWarning("The Proxy {0} timeout!", proxy);
        return Task.CompletedTask;
    }
}
