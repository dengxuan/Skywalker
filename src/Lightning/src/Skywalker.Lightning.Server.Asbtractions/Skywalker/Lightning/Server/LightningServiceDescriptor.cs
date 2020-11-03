using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skywalker.Lightning.Server
{
    public class LightningServiceDescriptor
    {

        /// <summary>
        ///     Name of the service
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Invoke service function:
        ///     IDictionary is parameters,
        ///     return object
        /// </summary>
        public Func<IDictionary<string, object>, Task<object>> InvokeHandler { get; set; }

        public LightningServiceDescriptor(string name, Func<IDictionary<string, object>, Task<object>> invokeHandler)
        {
            Name = name;
            InvokeHandler = invokeHandler;
        }
    }
}
