using Microsoft.Extensions.Logging;
using Skywalker.Lightning.Cluster;
using Skywalker.Lightning.Cluster.Internal;
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
        private readonly ILogger<PollingAddressSelector> _logger;

        private readonly ILightningDescriptorResolver _descriptorContainer;

        private readonly ConcurrentDictionary<string, Lazy<ServerIndexHolder>> _addresses = new ConcurrentDictionary<string, Lazy<ServerIndexHolder>>();

        public PollingAddressSelector(ILightningDescriptorResolver descriptorContainer, ILogger<PollingAddressSelector> logger)
        {
            _descriptorContainer = descriptorContainer;
            _logger = logger;
        }

        public override async Task<LightningAddress> GetAddressAsync(string serviceName)
        {
            LightningDescriptor? clusterDescriptor = _descriptorContainer.ResolveLightningDescriptor(serviceName);
            if (clusterDescriptor == null)
            {
                throw new Exception($"LightningDescriptor could't found by {serviceName}");
            }
            var serverIndexHolder = _addresses.GetOrAdd(serviceName, key => new Lazy<ServerIndexHolder>(() => new ServerIndexHolder()));
            IReadOnlyList<LightningAddress> addresses = clusterDescriptor.Addresses.ToList();
            do
            {
                addresses = clusterDescriptor.Addresses.ToList();
                await Task.Delay(1000);
            } while (addresses.Count == 0);
            LightningAddress? address = await serverIndexHolder.Value.GetAddressAsync(addresses);
            _logger.LogDebug($"{serviceName}, request address: {address?.IPEndPoint}");
            return address!.Value;
        }

        private class ServerIndexHolder
        {
            private int _lock = 0;

            private int _latestIndex;

            public Task<LightningAddress?> GetAddressAsync(IReadOnlyList<LightningAddress> addresses)
            {
                return Task.Run<LightningAddress?>(() =>
                {
                    if (addresses.Count == 0)
                    {
                        return null;
                    }
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
                });
            }
        }
    }
}
