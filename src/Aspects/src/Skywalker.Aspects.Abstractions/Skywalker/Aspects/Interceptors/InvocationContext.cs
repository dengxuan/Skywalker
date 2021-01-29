using Skywalker.Aspects.DynamicProxy;
using System.Threading.Tasks;

namespace Skywalker.Aspects.Interceptors
{
    /// <summary>
    /// Represents the invocation context specific to calling the proxy.
    /// </summary>
    public class InvocationContext
    {
        public IInvocation Invocation { get; }

        public InvocationContext(IInvocation invocation) => Invocation = invocation;

        public async Task ProceedAsync()
        {
            await Task.Run(() => Invocation.Proceed());
        }
    }
}
