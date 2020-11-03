using System.Collections.Generic;

namespace Skywalker.Lightning.Cluster.Abstractions
{
    public interface ILightningClusterDescriptor
    {
        string? Id { get; set; }

        List<ILightningCluster>? LightningClusters { get; set; }
    }
}
