using System;

namespace Skywalker.EventBus.Abstractions;

public interface IEventHandlerDisposeWrapper : IDisposable
{
    IEventHandler EventHandler { get; }
}
