using Skywalker.Lightning.Cluster.Abstractions;

namespace Skywalker.Lightning.Cluster
{
    public class LightningCluster : ILightningCluster
    {

        public string Address { get; set; }

        public int Port { get; set; }

        public int Weight { get; set; }

        public bool EnableTls { get; set; }

        public LightningCluster(string address, int port, int weight, bool enableTls)
        {
            Address = address;
            Port = port;
            Weight = weight;
            EnableTls = enableTls;
        }
    }
}
