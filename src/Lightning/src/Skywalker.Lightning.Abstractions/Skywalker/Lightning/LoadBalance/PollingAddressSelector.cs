using Microsoft.Extensions.Logging;
using Skywalker.Lightning.Cluster.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Lightning.LoadBalance
{
    public class PollingAddressSelector : AddressSelector
    {
        private readonly ConcurrentDictionary<string, Lazy<ServerIndexHolder>> _addresses = new ConcurrentDictionary<string, Lazy<ServerIndexHolder>>();

        private readonly ILogger<PollingAddressSelector> _logger;

        public PollingAddressSelector(ILogger<PollingAddressSelector> logger)
        {
            _logger = logger;
        }

        public override Task<ILightningCluster> GetAddressAsync(ILightningClusterDescriptor cluster, string serviceName)
        {
            var serverIndexHolder = _addresses.GetOrAdd(serviceName, key => new Lazy<ServerIndexHolder>(() => new ServerIndexHolder()));
            var address = serverIndexHolder.Value.GetAddress(cluster.LightningClusters!.ToList());
            _logger.LogDebug($"{cluster.Id}, request address: {address.Address}: {address.Port}");
            return Task.FromResult(address);
        }

        private class ServerIndexHolder
        {
            private int _latestIndex;
            private int _lock;

            public ILightningCluster GetAddress(List<ILightningCluster> addresses)
            {
                while (true)
                {
                    if (Interlocked.Exchange(ref _lock, 1) != 0)
                    {
                        default(SpinWait).SpinOnce();
                        continue;
                    }

                    _latestIndex = (_latestIndex + 1) % addresses.Count;
                    var address = addresses[_latestIndex];
                    Interlocked.Exchange(ref _lock, 0);
                    return address;
                }
            }
        }
    }
}
