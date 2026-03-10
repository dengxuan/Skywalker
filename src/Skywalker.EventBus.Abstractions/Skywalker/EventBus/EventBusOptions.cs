using Skywalker.EventBus.Abstractions;
using Skywalker.Extensions.Collections.Generic;

namespace Skywalker.EventBus;

public sealed class EventBusOptions
{
    internal TypeList<IEventHandler> Handlers { get; set; } = [];

    public void AddEventHandler<THandler>() where THandler : IEventHandler
    {
        var implemented = typeof(THandler).GetInterfaces().Any(i =>
        {
            return i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>);
        });

        if (!implemented)
        {
            throw new InvalidOperationException($"{typeof(THandler).FullName} must implement IEventHandler<TEvent>");
        }
        Handlers.Add<THandler>();
    }
}
