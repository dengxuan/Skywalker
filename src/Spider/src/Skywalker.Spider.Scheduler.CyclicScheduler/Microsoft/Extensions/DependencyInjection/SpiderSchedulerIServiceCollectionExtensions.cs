﻿using Skywalker.Spider.Abstractions;
using Skywalker.Spider.Scheduler.Abstractions;
using Skywalker.Spider.Schrduler;

namespace Microsoft.Extensions.DependencyInjection;

public static class SpiderSchedulerIServiceCollectionExtensions
{
    public static ISpiderBuilder CyclicScheduler(this ISpiderBuilder builder)
    {
        builder.AddDuplicateRemover();
        builder.Services.AddSingleton<IScheduler, QueueCyclicScheduler>();
        return builder;
    }
}
