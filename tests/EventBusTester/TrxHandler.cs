// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.EventBus;
using Skywalker.EventBus.Abstractions;

namespace EventBusTester;

public record TrxEto(string Id, string Name, decimal Amount);

internal class TrxHandler : IEventHandler<TrxEto>
{
    public Task HandleEventAsync(TrxEto eventData)
    {
        Console.WriteLine(eventData);
        return Task.CompletedTask;
    }
}
