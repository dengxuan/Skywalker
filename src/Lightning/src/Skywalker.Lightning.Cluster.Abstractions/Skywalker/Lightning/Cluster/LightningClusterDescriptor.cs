using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Skywalker.Lightning.Cluster
{
    public class LightningClusterDescriptor
    {
        private readonly ConcurrentDictionary<string, LightningAddress> _addresses = new ConcurrentDictionary<string, LightningAddress>();

        internal event Action<LightningAddress>? OnAdded;

        internal event Action<LightningAddress>? OnRemoved;

        internal ICollection<LightningAddress> Addresses
        {
            get
            {
                lock (_addresses)
                {
                    return _addresses.Values;
                }
            }
        }

        public LightningClusterDescriptor(LightningAddress address)
        {
            lock (_addresses)
            {
                _addresses.TryAdd(address.IPEndPoint.ToString(), address);
            }
        }

        public void AddLightingAddress(LightningAddress address)
        {
            lock (_addresses)
            {
                _addresses.TryAdd(address.Key, address);
            }
        }

        public void RemoveLightingAddress(string ipAddressWithPort)
        {
            lock (_addresses)
            {
                _addresses.TryRemove(ipAddressWithPort, out LightningAddress lightningAddress);
            }
        }
    }
}
