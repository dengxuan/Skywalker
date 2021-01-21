using System.Collections.Generic;

namespace Skywalker.Scheduler.Abstractions
{
    public interface IScheduleBuilderFactory
    {
        IReadOnlyCollection<IScheduleBuilder> GetScheduleBuilders();
    }
}
