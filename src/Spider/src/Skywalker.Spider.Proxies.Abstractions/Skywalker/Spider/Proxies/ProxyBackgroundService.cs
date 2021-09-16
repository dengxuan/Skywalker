using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.Spider.Proxies.Abstractions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Spider.Proxies
{
    public class ProxyBackgroundService : BackgroundService
    {
        private readonly ProxyOptions _options;
        private readonly IProxyPool _pool;
        private readonly IProxySupplier _proxySupplier;
        private readonly ILogger<ProxyBackgroundService> _logger;
        public ProxyBackgroundService(IProxyPool pool, IProxySupplier proxySupplier, IOptions<ProxyOptions> options, ILogger<ProxyBackgroundService> logger)
        {
            _pool = pool;
            _proxySupplier = proxySupplier;
            _options = options.Value;
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _pool.InitializeAsync();
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var proxies = await _proxySupplier.GetProxiesAsync();
                    var cnt = await _pool.SetAsync(proxies);
                    _logger.LogInformation($"Find {proxies.Count()} proxies, {cnt} are avaliable! ");
                }
                catch (Exception e)
                {
                    _logger.LogError($"Get proxies failed: {e}");
                }
                finally
                {
                    await Task.Delay(_options.RefreshInterval, stoppingToken);
                }
            }
        }
    }
}

