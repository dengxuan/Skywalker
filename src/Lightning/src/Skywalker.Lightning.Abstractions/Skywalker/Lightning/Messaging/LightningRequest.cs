using System.Collections.Generic;

namespace Skywalker.Lightning.Messaging
{
    public class LightningRequest
    {
        public string ServiceName { get; set; }

        /// <summary>
        /// Stores arbitrary metadata properties associated.
        /// </summary>
        public IDictionary<string, object> Parameters { get; }

        public LightningRequest(string serviceName, IDictionary<string, object> parameters)
        {
            ServiceName = serviceName;
            Parameters = parameters ?? new Dictionary<string, object>();
        }
    }
}
