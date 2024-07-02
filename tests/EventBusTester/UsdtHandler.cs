// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.EventBus.Abstractions;

namespace EventBusTester;

public record UsdtEto(string Id, string Name, decimal Amount);

internal class UsdtHandler : IEventHandler<UsdtEto>
{
    public Task HandleEventAsync(UsdtEto eventData)
    {
        Console.WriteLine(eventData);
        return Task.CompletedTask;
    }
}
