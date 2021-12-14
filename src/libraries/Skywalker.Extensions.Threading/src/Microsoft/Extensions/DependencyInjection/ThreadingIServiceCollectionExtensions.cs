using Skywalker.Extensions.Threading;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ThreadingIServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ICancellationTokenProvider>(NullCancellationTokenProvider.Instance);
            services.AddSingleton(typeof(IAmbientScopeProvider<>), typeof(AmbientDataContextAmbientScopeProvider<>));
            return services;
        }
    }
}
