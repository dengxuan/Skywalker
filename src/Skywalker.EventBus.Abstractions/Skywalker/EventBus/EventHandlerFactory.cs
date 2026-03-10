using Microsoft.Extensions.DependencyInjection;
using Skywalker.EventBus.Abstractions;

namespace Skywalker.EventBus;

/// <summary>
/// This <see cref="IEventHandlerFactory"/> implementation is used to handle events
/// by a transient instance object. 
/// </summary>
/// <remarks>
/// This class always creates a new transient instance of the handler type.
/// </remarks>
public class EventHandlerFactory(IServiceProvider serviceProvider) : IEventHandlerFactory
{
    /// <summary>
    /// Creates a new instance of the handler object.
    /// </summary>
    /// <returns>The handler object</returns>
    public IEventHandler GetHandler(Type handlerType)
    {
        var handler = (IEventHandler)ActivatorUtilities.CreateInstance(serviceProvider, handlerType);
        return handler;
    }
}
