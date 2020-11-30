using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;

namespace Skywalker.Lightning.Cluster.Internal
{
    internal class LightningDescriptorContainer : ILightningDescriptorContainer, ILightningDescriptorResolver
    {
        private static readonly object _locker = new object();

        private readonly ConcurrentDictionary<string, LightningDescriptor> _lightningDescriptors = new ConcurrentDictionary<string, LightningDescriptor>();

        public void AddLightningAddress(string serviceName, LightningAddress lightningAddress)
        {
            lock (_locker)
            {
                if (_lightningDescriptors.TryGetValue(serviceName, out LightningDescriptor? clusterDescriptor))
                {
                    clusterDescriptor.AddLightingAddress(lightningAddress);
                }
                else
                {
                    _lightningDescriptors.TryAdd(serviceName, new LightningDescriptor(lightningAddress));
                }
            }
        }

        public void RemoveLightningAddress(string serviceName, IPEndPoint endPoint)
        {
            lock (_locker)
            {
                if (_lightningDescriptors.TryGetValue(serviceName, out LightningDescriptor? clusterDescriptor))
                {
                    clusterDescriptor.RemoveLightingAddress(endPoint.ToString());
                }
            }
        }

        public LightningDescriptor ResolveLightningDescriptor(string serviceName)
        {
            lock (_locker)
            {
                if (_lightningDescriptors.TryGetValue(serviceName, out LightningDescriptor? clusterDescriptor))
                {
                    return clusterDescriptor;
                }
                throw new System.Exception($"LightningDescriptor could't found by {serviceName}");
            }
        }
    }
}
