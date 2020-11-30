using System.Collections.Generic;

namespace Skywalker.Lightning
{
    public class LightningInvokeContext
    {
        public string ServiceName { get; set; }

        /// <summary>
        /// Stores arbitrary metadata properties associated.
        /// </summary>
        public IDictionary<string, object> Parameters { get; }

        public LightningInvokeContext(string serviceName, IDictionary<string, object> parameters)
        {
            ServiceName = serviceName;
            Parameters = parameters;
        }
    }
}
