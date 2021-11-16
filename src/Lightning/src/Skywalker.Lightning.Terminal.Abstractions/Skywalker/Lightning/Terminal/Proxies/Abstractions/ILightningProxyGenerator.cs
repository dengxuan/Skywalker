using System;
using System.Collections.Generic;

namespace Skywalker.Lightning.Terminal.Proxies.Abstractions
{
    public interface ILightningProxyGenerator
    {
        /// <summary>
        ///     generate proxy for types
        /// </summary>
        /// <param name="interfaceTypes"></param>
        /// <returns></returns>
        IEnumerable<Type> GenerateProxy(IEnumerable<Type> interfaceTypes);

        /// <summary>
        ///     get all generated service proxy
        /// </summary>
        /// <returns></returns>
        IEnumerable<Type> GetGeneratedServiceProxyTypes();
    }
}
