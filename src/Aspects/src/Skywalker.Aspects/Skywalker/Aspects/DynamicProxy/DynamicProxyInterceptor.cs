using Microsoft.Extensions.Logging;
using Skywalker.Aspects.Interceptors;
using System.Threading.Tasks;

namespace Skywalker.Aspects.DynamicProxy
{
    internal class DynamicProxyInterceptor : IInterceptor
    {
        private readonly InterceptorDelegate _interceptor;
        private readonly ILogger<DynamicProxyInterceptor> _logger;

        public DynamicProxyInterceptor(InterceptorDelegate inteceptor, ILogger<DynamicProxyInterceptor> logger)
        {
            _interceptor = inteceptor;
            _logger = logger;
        }
        public void Intercept(IInvocation invocation)
        {
            _logger.LogInformation("Begin DynamicProxyInterceptor [{0}]", invocation.InvocationTarget.ToString());
            var intercepterDelegate = _interceptor(Next);
            intercepterDelegate(new InvocationContext(invocation));
            _logger.LogInformation("End DynamicProxyInterceptor [{0}]", invocation.InvocationTarget.ToString());
        }
        private async Task Next(InvocationContext context)
        {
            _logger.LogInformation("Begin ProceedAsync [{0}]", context.Invocation.ToString());
            await context.ProceedAsync();
            _logger.LogInformation("End ProceedAsync [{0}]", context.Invocation.ToString());
        }
    }
}
