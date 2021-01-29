using Skywalker.Aspects.DynamicProxy;
using Skywalker.Aspects.Interceptors;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AspectsServiceCollectionExtensions
    {
        public static IServiceCollection AddAspects(this IServiceCollection services)
        {
            services.AddSingleton<IProxyGenerator, ProxyGenerator>();
            services.AddSingleton<IInterceptorFactory, InterceptorFactory>();
            services.AddSingleton<IInterceptorChainBuilder, InterceptorChainBuilder>();
            services.AddSingleton<InvocationContext>();
            return services;
        }
    }
}
