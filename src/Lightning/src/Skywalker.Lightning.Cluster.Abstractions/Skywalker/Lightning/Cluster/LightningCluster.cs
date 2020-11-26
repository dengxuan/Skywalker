using Skywalker.Lightning.Cluster.Abstractions;
using System;
using System.Collections.Concurrent;

namespace Skywalker.Lightning.Cluster
{
    public class LightningCluster : ILightningCluster
    {
        private readonly ConcurrentDictionary<string, LightningClusterDescriptor> _clusterDescriptors = new ConcurrentDictionary<string, LightningClusterDescriptor>();

        public void DeregisterAsync(string name, LightningAddress address)
        {
            lock (_clusterDescriptors)
            {
                if (_clusterDescriptors.TryGetValue(name, out LightningClusterDescriptor? clusterDescriptor))
                {
                    clusterDescriptor.RemoveLightingAddress(address.Key);
                }
            }
        }

        public void RegisterAsync(string name, LightningAddress address)
        {
            lock (_clusterDescriptors)
            {
                if (_clusterDescriptors.TryGetValue(name, out LightningClusterDescriptor? clusterDescriptor))
                {
                    clusterDescriptor.AddLightingAddress(address);
                }
                else
                {
                    _clusterDescriptors.TryAdd(name, new LightningClusterDescriptor(address));
                }
            }
        }

        internal LightningClusterDescriptor? GetLightningClusterDescriptor(string serviceName)
        {
            lock (_clusterDescriptors)
            {
                _clusterDescriptors.TryGetValue(serviceName, out LightningClusterDescriptor? clusterDescriptor);
                return clusterDescriptor;
            }
        }

        void ILightningCluster.DeregisterAsync(string name, LightningAddress address)
        {
            throw new NotImplementedException();
        }

        LightningClusterDescriptor? ILightningCluster.GetLightningClusterDescriptor(string serviceName)
        {
            throw new NotImplementedException();
        }

        void ILightningCluster.RegisterAsync(string name, LightningAddress endPoint)
        {
            throw new NotImplementedException();
        }
    }
}
