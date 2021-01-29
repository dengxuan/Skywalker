using System;

namespace Skywalker.Aspects.Interceptors
{
    public interface IInterceptorFactory
    {
        object CreateProxy(Type typeToProxy, object target);
    }
}
