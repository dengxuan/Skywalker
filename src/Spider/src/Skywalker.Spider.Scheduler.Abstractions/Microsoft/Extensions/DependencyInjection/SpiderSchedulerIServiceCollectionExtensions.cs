using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Spider.Abstractions;
using Skywalker.Spider.Scheduler;
using Skywalker.Spider.Scheduler.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class SpiderSchedulerIServiceCollectionExtensions
{
    public static ISpiderBuilder AddScheduler(this ISpiderBuilder builder)
    {
        builder.AddDuplicateRemover();
        builder.Services.TryAddSingleton<IScheduler, DefaultScheduler>();
        return builder;
    }
}
