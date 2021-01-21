using System.ComponentModel;

namespace Skywalker.Scheduler
{
    /// <summary>
    /// 关于<seealso cref="IWorker"/>的状态信息。
    /// </summary>
    public enum WorkerState
    {
        /// <summary>未运行/已停止。</summary>
        [Description("${Text.WorkerState.Stopped}")]
        Stopped = 0,

        /// <summary>运行中。</summary>
        [Description("${Text.WorkerState.Running}")]
        Running = 1,

        /// <summary>正在启动中。</summary>
        [Description("${Text.WorkerState.Starting}")]
        Starting,

        /// <summary>正在停止中。</summary>
        [Description("${Text.WorkerState.Stopping}")]
        Stopping,

        /// <summary>正在暂停中。</summary>
        [Description("${Text.WorkerState.Pausing}")]
        Pausing,

        /// <summary>已经暂停。</summary>
        [Description("${Text.WorkerState.Paused}")]
        Paused,

        /// <summary>正在恢复中。</summary>
        [Description("${Text.WorkerState.Resuming}")]
        Resuming,
    }
}
