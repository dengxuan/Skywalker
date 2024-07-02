// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.EventBus;

internal class EventHandlerInvokerCacheItem
{
    public IEventHandlerMethodExecutor? Local { get; set; }

    public IEventHandlerMethodExecutor? Distributed { get; set; }
}
