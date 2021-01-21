using Skywalker.Scheduler.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Skywalker.Scheduler
{
    public class HandlerContext : IHandlerContext
    {
        private IDictionary<string, object> _parameters;

        public HandlerContext(IScheduler scheduler, ITrigger trigger, string scheduleId, DateTime timestamp, int index)
        {
            this.Index = index;
            this.Timestamp = timestamp;
            this.ScheduleId = scheduleId;
            this.Scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            this.Trigger = trigger ?? throw new ArgumentNullException(nameof(trigger));
        }

        public int Index { get; }

        public string ScheduleId { get; }

        public DateTime Timestamp { get; }

        public HandlerFailure? Failure { get; set; }

        public IScheduler Scheduler { get; }

        public ITrigger Trigger { get; }

        public bool HasParameters
        {
            get
            {
                return _parameters != null && _parameters.Count > 0;
            }
        }

        public IDictionary<string, object> Parameters
        {
            get
            {
                if(_parameters == null)
                {

                    Interlocked.CompareExchange(ref _parameters, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), null);
                }

                return _parameters;
            }
        }
    }
}
