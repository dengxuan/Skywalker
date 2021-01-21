using System;

namespace Skywalker.Scheduler
{
    public class OccurringEventArgs : EventArgs
    {
        public OccurringEventArgs(string scheduleId)
        {
            this.ScheduleId = scheduleId;
        }

        public string ScheduleId { get; }
    }
}
