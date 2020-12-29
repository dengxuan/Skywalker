using Skywalker.Aspects.Interceptors;
using System.Threading.Tasks;

namespace Skywalker.Aspects.DynamicProxy
{
    internal class DynamicProxyInterceptor : IInterceptor
    {
        private readonly InterceptorDelegate _interceptor;

        public DynamicProxyInterceptor(InterceptorDelegate inteceptor)
        {
            _interceptor = inteceptor;
        }
        public void Intercept(IInvocation invocation)
        {
            static Task next(InvocationContext context) => (context).ProceedAsync();
            var intercepterDelegate = _interceptor(next);
            _interceptor(next)(new InvocationContext(invocation)).Wait();
        }
    }
}
