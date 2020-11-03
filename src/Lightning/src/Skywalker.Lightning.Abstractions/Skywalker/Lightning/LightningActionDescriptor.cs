using System.Collections.Generic;

namespace Skywalker.Lightning
{
    public class LightningActionDescriptor
    {
        public string Id { get; }

        /// <summary>
        /// Stores arbitrary metadata properties associated with the <see cref="LightningServiceParameterDescriptor"/>.
        /// </summary>
        public Dictionary<string,object> Parameters { get; }

        public LightningActionDescriptor(string id, Dictionary<string, object> parameters)
        {
            Id = id;
            Parameters = parameters ?? new Dictionary<string,object>();
        }
    }
}
