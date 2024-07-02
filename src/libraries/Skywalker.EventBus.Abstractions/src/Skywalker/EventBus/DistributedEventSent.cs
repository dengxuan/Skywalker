// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.EventBus;

public class DistributedEventSent
{
    public DistributedEventSource Source { get; set; }

    public string EventName { get; set; } = default!;

    public object EventData { get; set; } = default!;
}
