using Skywalker.Spider.Abstractions;
using Skywalker.Spider.Scheduler;
using Skywalker.Spider.Scheduler.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class SpiderSchedulerIServiceCollectionExtensions
{
    public static ISpiderBuilder BreadthFirstScheduler(this ISpiderBuilder builder)
    {
        builder.AddDuplicateRemover();
        builder.Services.AddSingleton<IScheduler, QueueBreadthFirstScheduler>();
        return builder;
    }
}
