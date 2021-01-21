namespace Skywalker.Scheduler.Abstractions
{
    public interface IScheduleBuilder
    {
        IHandler Handler { get; }

        ITrigger Trigger { get; }
    }
}
