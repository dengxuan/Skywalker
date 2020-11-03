namespace Skywalker.Lightning.Cluster.Abstractions
{
    public interface ILightningCluster
    {
        public int Weight { get; set; }

        public string Address { get; set; }

        public int Port { get; set; }

        public bool EnableTls { get; set; }
    }
}
