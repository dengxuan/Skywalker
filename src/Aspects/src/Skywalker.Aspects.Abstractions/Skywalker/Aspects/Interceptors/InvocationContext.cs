using Skywalker.Aspects.DynamicProxy;
using System.Reflection;
using System.Threading.Tasks;

namespace Skywalker.Aspects.Interceptors
{
    /// <summary>
    /// Represents the invocation context specific to calling the proxy.
    /// </summary>
    public class InvocationContext
    {

        private MethodInfo? methodInfo = typeof(InvocationContext).GetMethod("HandleAsync", BindingFlags.Instance | BindingFlags.Public);

        private void HandleAsyncWithReflection(IInvocation invocation)
        {
            var resultType = invocation.Method.ReturnType.GetGenericArguments()[0];
            var mi = methodInfo?.MakeGenericMethod(resultType);
            invocation.ReturnValue = mi.Invoke(this, new[] { invocation.ReturnValue });
        }

        //构造等待返回值的异步方法
        public async Task<T> HandleAsync<T>(Task<T> task)
        {
            var t = await task;

            //Do arter

            return t;
        }

        public IInvocation Invocation { get; }

        public InvocationContext(IInvocation invocation) => Invocation = invocation;

        public async Task ProceedAsync()
        {
            Invocation.Proceed();
            var returnType = Invocation.Method.ReflectedType;     //获取被代理方法的返回类型
            if (returnType != null && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                HandleAsyncWithReflection(Invocation);
            }
        }
    }
}
