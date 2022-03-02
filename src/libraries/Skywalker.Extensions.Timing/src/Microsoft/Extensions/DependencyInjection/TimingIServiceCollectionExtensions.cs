using Skywalker.Extensions.Timing;

namespace Microsoft.Extensions.DependencyInjection;

public static class TimingIServiceCollectionExtensions
{
    internal static IServiceCollection AddTimingServices(this IServiceCollection services)
    {
        services.AddSingleton<IClock, Clock>();
        services.AddSingleton<ITimezoneProvider, TZConvertTimezoneProvider>();
        return services;
    }

    public static IServiceCollection AddTiming(this IServiceCollection services)
    {
        return services.AddTimingServices();
    }

    public static IServiceCollection AddTiming(this IServiceCollection services, Action<SkywalkerClockOptions> options)
    {
        services.Configure(options);
        return services.AddTimingServices();
    }
}
