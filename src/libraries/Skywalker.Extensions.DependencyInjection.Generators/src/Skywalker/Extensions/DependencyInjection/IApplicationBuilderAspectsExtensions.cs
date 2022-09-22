using Microsoft.Extensions.Hosting;
using Skywalker.Extensions.DependencyInjection.Interceptors.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;


public static class IApplicationBuilderAspectsExtensions
{
    public static IHost UseAspects(this IHost builder, Action<IInterceptorChainBuilder> options)
    {
        var interceptorChainBuilder = builder.Services.GetRequiredService<IInterceptorChainBuilder>();
        options?.Invoke(interceptorChainBuilder);
        return builder;
    }
}
