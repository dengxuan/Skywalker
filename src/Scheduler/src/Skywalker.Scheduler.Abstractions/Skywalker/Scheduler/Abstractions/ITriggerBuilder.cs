using System;

namespace Skywalker.Scheduler.Abstractions
{
    public interface ITriggerBuilder
    {
        ITrigger Build(string expression, DateTime? expiration = null, DateTime? effective = null);
    }
}
