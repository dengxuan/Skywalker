using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Skywalker.Aspects.DynamicProxy
{
    internal class DynamicProxyInterceptorSelector : IInterceptorSelector
    {
        private readonly IDictionary<MethodInfo, IInterceptor> _interceptors;

        public DynamicProxyInterceptorSelector(IDictionary<MethodInfo, IInterceptor> interceptors)
        {
            _interceptors = interceptors;
        }

        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            if (_interceptors.TryGetValue(method, out IInterceptor? interceptor) && interceptors.Contains(interceptor))
            {
                return new IInterceptor[] { interceptor };
            }
            return Array.Empty<IInterceptor>();
        }
    }
}
