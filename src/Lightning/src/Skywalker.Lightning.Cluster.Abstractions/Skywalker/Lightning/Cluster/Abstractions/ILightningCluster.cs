namespace Skywalker.Lightning.Cluster.Abstractions
{
    public interface ILightningCluster
    {
        void RegisterAsync(string name, LightningAddress endPoint);

        void DeregisterAsync(string name, LightningAddress address);

        internal LightningClusterDescriptor? GetLightningClusterDescriptor(string serviceName);
    }
}
