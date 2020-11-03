using Skywalker.Lightning.Abstractions;
using Skywalker.Lightning.Proxies.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Skywalker.Lightning.Proxies
{
    public class LightningProxyFactory : ILightningProxyFactory
    {
        private readonly ILightningInvoker _LightningInvoker;
        private readonly IList<Type> _serviceProxyTypes;

        public LightningProxyFactory(ILightningProxyGenerator generator, ILightningInvoker LightningInvoker)
        {
            _serviceProxyTypes = generator.GetGeneratedServiceProxyTypes().ToList();
            _LightningInvoker = LightningInvoker;
        }

        public T GetService<T>() where T : class
        {
            var proxyType = _serviceProxyTypes.Single(typeof(T).GetTypeInfo().IsAssignableFrom);
            var instance = proxyType.GetTypeInfo().GetConstructors().First().Invoke(new object[]
            {
                _LightningInvoker
            });
            return instance as T;
        }
    }
}
