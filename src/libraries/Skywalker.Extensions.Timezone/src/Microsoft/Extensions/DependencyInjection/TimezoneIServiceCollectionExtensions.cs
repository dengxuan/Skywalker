using Skywalker.Extensions.Timezone;

namespace Microsoft.Extensions.DependencyInjection;

public static class TimezoneIServiceCollectionExtensions
{
    internal static IServiceCollection AddTimezoneServices(this IServiceCollection services)
    {
        services.AddSingleton<IClock, Clock>();
        services.AddSingleton<ITimezoneProvider, TZConvertTimezoneProvider>();
        return services;
    }

    public static IServiceCollection AddTimezone(this IServiceCollection services)
    {
        return services.AddTimezoneServices();
    }

    public static IServiceCollection AddTimezone(this IServiceCollection services, Action<TimezoneOptions> options)
    {
        services.Configure(options);
        return services.AddTimezoneServices();
    }
}
