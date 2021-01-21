using Microsoft.Extensions.DependencyInjection;
using Skywalker.Scheduler.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skywalker.Scheduler
{
    public class ScheduleBuilderFactory : IScheduleBuilderFactory
    {
        private readonly IServiceProvider _iocResolver;

        public ScheduleBuilderFactory(IServiceProvider iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public IReadOnlyCollection<IScheduleBuilder> GetScheduleBuilders()
        {
            return _iocResolver.GetServices<IScheduleBuilder>().ToList();
        }
    }
}
