using System;
using System.Collections.Generic;

namespace Skywalker.Scheduler.Abstractions
{
    /// <summary>
    /// 表示调度处理上下文的接口。
    /// </summary>
    public interface IHandlerContext
    {
        /// <summary>
        /// 获取调度事务中的处理序号。
        /// </summary>
        int Index { get; }

        /// <summary>
        /// 获取调度任务编号。
        /// </summary>
        string ScheduleId { get; }

        /// <summary>
        /// 获取任务首次调度的时间。
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// 获取处理失败的扩展信息。
        /// </summary>
        HandlerFailure? Failure { get; set; }

        /// <summary>
        /// 获取处理的调度器对象。
        /// </summary>
        IScheduler Scheduler { get; }

        /// <summary>
        /// 获取关联的触发器对象。
        /// </summary>
        ITrigger Trigger { get; }

        /// <summary>
        /// 获取一个值，指示上下文是否含有扩展参数。
        /// </summary>
        bool HasParameters { get; }

        /// <summary>
        /// 获取上下文的扩展参数集。
        /// </summary>
        IDictionary<string, object> Parameters { get; }
    }
}
