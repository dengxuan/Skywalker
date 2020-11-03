using System;

namespace Skywalker.EventBus
{
    public interface IEventNameProvider
    {
        string GetName(Type eventType);
    }
}