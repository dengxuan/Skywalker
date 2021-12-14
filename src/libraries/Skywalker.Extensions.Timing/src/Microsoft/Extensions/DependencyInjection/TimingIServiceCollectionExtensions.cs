using Skywalker.Extensions.Timing;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TimingIServiceCollectionExtensions
    {
        public static IServiceCollection AddTiming(this IServiceCollection services)
        {
            services.AddSingleton<IClock, Clock>();
            services.AddSingleton<ITimezoneProvider, TZConvertTimezoneProvider>();
            return services;
        }
    }
}
