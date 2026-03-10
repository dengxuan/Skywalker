// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.EventBus;

internal sealed class EventBusBuilder(IServiceCollection services) : IEventBusBuilder
{
    public IServiceCollection Services => services;
}
