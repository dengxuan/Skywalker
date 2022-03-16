using Skywalker.Extensions.Threading;

namespace Microsoft.Extensions.DependencyInjection;

public static class ThreadingIServiceCollectionExtensions
{
    public static IServiceCollection AddThreading(this IServiceCollection services)
    {
        services.AddSingleton<IAsyncQueryableExecuter, AsyncQueryableExecuter>();
        services.AddSingleton<ICancellationTokenProvider>(NullCancellationTokenProvider.Instance);
        services.AddSingleton(typeof(IAmbientScopeProvider<>), typeof(AmbientDataContextAmbientScopeProvider<>));
        return services;
    }
}
