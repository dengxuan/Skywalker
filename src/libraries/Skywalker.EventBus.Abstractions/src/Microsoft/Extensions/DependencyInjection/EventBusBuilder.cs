using Skywalker.EventBus;
using Skywalker.EventBus.Abstractions;
using Skywalker.Extensions.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection;

public class EventBusBuilder
{
    public IServiceCollection Services { get; }

    public ITypeList<IEventHandler> Handlers { get; } = new TypeList<IEventHandler>();

    public EtoMappingDictionary EtoMappings { get; } = [];

    internal EventBusBuilder(IServiceCollection services)
    {
        Services = services;
    }
}
