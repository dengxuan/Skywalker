using Skywalker.Lightning.Cluster.Abstractions;
using System.Collections.Generic;

namespace Skywalker.Lightning
{
    public class LightningInvokeContext
    {
        public ILightningCluster Cluster { get; }

        public string ServiceName { get; set; }

        /// <summary>
        /// Stores arbitrary metadata properties associated.
        /// </summary>
        public IDictionary<string, object> Parameters { get; }

        public LightningInvokeContext(ILightningCluster clusterDescriptor, string serviceName, IDictionary<string, object> parameters)
        {
            Cluster = clusterDescriptor;
            ServiceName = serviceName;
            Parameters = parameters;
        }
    }
}
