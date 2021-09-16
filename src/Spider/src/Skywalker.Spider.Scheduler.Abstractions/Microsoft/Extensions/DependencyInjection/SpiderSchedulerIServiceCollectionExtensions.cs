using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Spider.Scheduler;
using Skywalker.Spider.Scheduler.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class SpiderSchedulerIServiceCollectionExtensions
{
    public static IServiceCollection AddScheduler(this IServiceCollection services)
    {
        services.TryAddSingleton<IScheduler, DefaultScheduler>();
        return services;
    }
}
