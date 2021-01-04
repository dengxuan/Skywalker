using Skywalker.Aspects.Interceptors;

namespace Skywalker.Ddd.UnitOfWork
{
    internal class UnitOfWorkProvider : IInterceptorProvider
    {
        public void Use(IInterceptorChainBuilder builder)
        {
            builder.Use<UnitOfWorkInterceptor>(1);
        }
    }
}
