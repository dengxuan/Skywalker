using System.Threading.Tasks;

namespace Skywalker.EventBus.Abstractions;

/// <summary>
/// Undirect base interface for all event handlers.
/// <see cref="IEventHandler{TEvent}"/> instead of this one.
/// </summary>
public interface IEventHandler
{

}

public interface IEventHandler<in TEvent> : IEventHandler
{
    /// <summary>
    /// Handler handles the event by implementing this method.
    /// </summary>
    /// <param name="eventData">Event data</param>
    Task HandleEventAsync(TEvent eventData);
}
