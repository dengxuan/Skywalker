using Skywalker.Spider.Scheduler;
using Skywalker.Spider.Scheduler.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class SpiderSchedulerIServiceCollectionExtensions
{
    public static IServiceCollection DepthFirstScheduler(this IServiceCollection services)
    {
        services.AddDuplicateRemover();
        services.AddSingleton<IScheduler, QueueDepthFirstScheduler>();
        return services;
    }
}
