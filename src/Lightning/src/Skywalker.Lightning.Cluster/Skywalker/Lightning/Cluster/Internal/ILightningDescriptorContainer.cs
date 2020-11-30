using System.Net;

namespace Skywalker.Lightning.Cluster
{
    public interface ILightningDescriptorContainer
    {
        void AddLightningAddress(string serviceName, LightningAddress lightningAddress);

        void RemoveLightningAddress(string serviceName, IPEndPoint endPoint);
    }
}
