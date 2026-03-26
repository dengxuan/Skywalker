using Microsoft.Extensions.DependencyInjection;
using Skywalker.Extensions.Timezone;


namespace Microsoft.Extensions.DependencyInjection;

public static class TimezoneIServiceCollectionExtensions
{
    public static IServiceCollection AddTimezone(this IServiceCollection services)
    {
        services.AddAutoServices();
        return services;
    }

    public static IServiceCollection AddTimezone(this IServiceCollection services, Action<TimezoneOptions> options)
    {
        services.Configure(options);
        services.AddAutoServices();
        return services;
    }
}
