using System.Collections.Generic;
using System.Net;

namespace Skywalker.Lightning.Cluster
{
    public class ClusterNodes
    {
        public List<IPEndPoint> EndPoints { get; set; }

        public ClusterNodes(List<IPEndPoint> endPoints)
        {
            EndPoints = endPoints;
        }
    }
}
