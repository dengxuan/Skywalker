using Microsoft.Extensions.Logging;
using Skywalker.Lightning.Cluster;
using Skywalker.Lightning.Cluster.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Skywalker.Lightning.LoadBalance
{
    public class PollingAddressSelector : AddressSelector
    {
        private readonly ILogger<PollingAddressSelector> _logger;

        private readonly ConcurrentDictionary<string, Lazy<ServerIndexHolder>> _addresses = new ConcurrentDictionary<string, Lazy<ServerIndexHolder>>();

        public PollingAddressSelector(ILightningCluster lightningCluster, ILogger<PollingAddressSelector> logger):base(lightningCluster)
        {
            _logger = logger;
        }

        public override LightningAddress? GetAddressAsync(string serviceName)
        {
            LightningClusterDescriptor? clusterDescriptor = LightningCluster.GetLightningClusterDescriptor(serviceName);
            if (clusterDescriptor == null)
            {
                return null;
            }
            var serverIndexHolder = _addresses.GetOrAdd(serviceName, key => new Lazy<ServerIndexHolder>(() => new ServerIndexHolder()));
            var address = serverIndexHolder.Value.GetAddress(clusterDescriptor.Addresses!.ToList());
            _logger.LogDebug($"{serviceName}, request address: {address.IPEndPoint}");
            return address;
        }

        private class ServerIndexHolder
        {
            private int _latestIndex;
            private int _lock;

            public LightningAddress GetAddress(List<LightningAddress> endPoints)
            {
                while (true)
                {
                    if (Interlocked.Exchange(ref _lock, 1) != 0)
                    {
                        default(SpinWait).SpinOnce();
                        continue;
                    }

                    _latestIndex = (_latestIndex + 1) % endPoints.Count;
                    var address = endPoints[_latestIndex];
                    Interlocked.Exchange(ref _lock, 0);
                    return address;
                }
            }
        }
    }
}
