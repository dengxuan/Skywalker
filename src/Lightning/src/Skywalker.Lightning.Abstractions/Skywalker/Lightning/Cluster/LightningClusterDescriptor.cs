using Skywalker.Lightning.Cluster.Abstractions;
using System.Collections.Generic;

namespace Skywalker.Lightning.Cluster
{
    public class LightningClusterDescriptor : ILightningClusterDescriptor
    {
        public string? Id { get; set; }

        public List<ILightningCluster>? LightningClusters { get; set; }
    }
}
