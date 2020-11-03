using System;
using System.Collections.Generic;
using System.Reflection;

namespace Skywalker.Lightning.Server.Abstractions
{
    public interface ILightningServiceFactory
    {
        event Action<List<LightningServiceDescriptor>> OnServiceLoaded;

        /// <summary>
        ///     load service
        /// </summary>
        /// <returns></returns>
        void LoadServices(List<Assembly> assemblies);

        /// <summary>
        /// Get all Lightning service descriptor
        /// </summary>
        /// <returns>all Lightning service descriptor</returns>
        List<LightningServiceDescriptor> GetLightningServiceDescriptors();

        /// <summary>
        /// Get Lightning service descriptor by name.
        /// </summary>
        /// <param name="name">Lightning service name</param>
        /// <returns>Lightning service descriptor or null</returns>
        LightningServiceDescriptor GetLightningServiceDescriptor(string name);
    }
}
