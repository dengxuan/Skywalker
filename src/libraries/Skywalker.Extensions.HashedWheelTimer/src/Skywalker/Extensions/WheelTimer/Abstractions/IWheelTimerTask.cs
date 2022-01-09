using System.Threading.Tasks;

namespace Skywalker.Extensions.WheelTimer.Abstractions
{
    /// <summary>
    /// A task which is executed after the delay specified with Timer.NewTimeout(TimerTask, long, TimeUnit).
    /// </summary>
    public interface IWheelTimerTask
    {
        /// <summary>
        /// Executed after the delay specified with Timer.NewTimeout(TimerTask, long, TimeUnit)
        /// </summary>
        /// <param name="timeout">timeout a handle which is associated with this task</param>
        Task RunAsync(IWheelTimeout timeout);
    }
}
