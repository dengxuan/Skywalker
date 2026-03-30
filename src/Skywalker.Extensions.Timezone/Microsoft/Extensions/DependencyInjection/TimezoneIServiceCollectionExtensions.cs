using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Extensions.Timezone;


namespace Microsoft.Extensions.DependencyInjection;

public static class TimezoneIServiceCollectionExtensions
{
    public static IServiceCollection AddTimezone(this IServiceCollection services)
    {
        services.TryAddSingleton<IClock, Clock>();
        services.TryAddSingleton<ITimezoneProvider, TZConvertTimezoneProvider>();
        return services;
    }

    public static IServiceCollection AddTimezone(this IServiceCollection services, Action<TimezoneOptions> options)
    {
        services.Configure(options);
        services.TryAddSingleton<IClock, Clock>();
        services.TryAddSingleton<ITimezoneProvider, TZConvertTimezoneProvider>();
        return services;
    }
}
