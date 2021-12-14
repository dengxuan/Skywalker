using System;

namespace Skywalker.EventBus.Abstractions;

public interface IEventNameProvider
{
    string GetName(Type eventType);
}
