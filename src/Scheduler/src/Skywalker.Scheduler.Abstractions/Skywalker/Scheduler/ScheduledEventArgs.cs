using Skywalker.Scheduler.Abstractions;
using System;

namespace Skywalker.Scheduler
{
    public class ScheduledEventArgs : EventArgs
    {
        public ScheduledEventArgs(string scheduleId, int count, ITrigger[] triggers)
        {
            this.Count = count;
            this.Triggers = triggers;
            this.ScheduleId = scheduleId;
        }

        public string ScheduleId { get; }

        public int Count { get; }

        public ITrigger[] Triggers { get; }
    }
}
