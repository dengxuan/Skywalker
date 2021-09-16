using Skywalker.Spider.Scheduler;
using Skywalker.Spider.Scheduler.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class SpiderSchedulerIServiceCollectionExtensions
{
    public static IServiceCollection BreadthFirstScheduler(this IServiceCollection services)
    {
        services.AddDuplicateRemover();
        services.AddSingleton<IScheduler, QueueBreadthFirstScheduler>();
        return services;
    }
}
