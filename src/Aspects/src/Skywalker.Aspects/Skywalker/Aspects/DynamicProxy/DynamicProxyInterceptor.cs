using Microsoft.Extensions.Logging;
using Skywalker.Aspects.Interceptors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Aspects.DynamicProxy
{
    //internal class DynamicProxyInterceptor : IInterceptor
    //{
    //    private readonly InterceptorDelegate _interceptor;
    //    private readonly ILogger<DynamicProxyInterceptor> _logger;

    //    public DynamicProxyInterceptor(InterceptorDelegate inteceptor, ILogger<DynamicProxyInterceptor> logger)
    //    {
    //        _interceptor = inteceptor;
    //        _logger = logger;
    //    }
    //    public void Intercept(IInvocation invocation)
    //    {
    //        _logger.LogInformation("Begin DynamicProxyInterceptor [{0}]", invocation.InvocationTarget.ToString());
    //        var intercepterDelegate = _interceptor(Next);
    //        intercepterDelegate(new InvocationContext(invocation)).Wait();
    //        _logger.LogInformation("End DynamicProxyInterceptor", invocation.InvocationTarget.ToString());
    //    }

    //    private async Task Next(InvocationContext context)
    //    {
    //        _logger.LogInformation("Begin ProceedAsync [{0}]", context.Invocation.ToString());
    //        await context.ProceedAsync();
    //        _logger.LogInformation("End ProceedAsync [{0}]", context.Invocation.ToString());
    //    }
    //}

    internal class DynamicProxyInterceptor : AsyncInterceptorBase
    {
        private InterceptorDelegate _interceptor;
        private readonly ILogger<DynamicProxyInterceptor> _logger;
        public DynamicProxyInterceptor(InterceptorDelegate inteceptor, ILogger<DynamicProxyInterceptor> logger)
        {
            _interceptor = inteceptor; 
            _logger = logger;
        }

        protected override Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            var invocationContext = new DynamicProxyInvocationContext(invocation);
            Task next(InvocationContext context) => proceed(invocation, proceedInfo);
            return _interceptor(next)(invocationContext);
        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            var invocationContext = new DynamicProxyInvocationContext(invocation);
            InterceptDelegate next = context => proceed(invocation, proceedInfo);
            await _interceptor(next)(invocationContext);
            if (invocationContext.ReturnValue is TResult)
            {
                return (TResult)invocationContext.ReturnValue;
            }
            return ((Task<TResult>)invocationContext.ReturnValue).Result;
        }
    }
}
