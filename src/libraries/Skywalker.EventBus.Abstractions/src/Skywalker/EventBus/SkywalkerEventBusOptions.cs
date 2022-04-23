using Microsoft.Extensions.DependencyInjection;
using Skywalker.EventBus.Abstractions;
using Skywalker.Extensions.Collections.Generic;

namespace Skywalker.EventBus;

public class SkywalkerEventBusOptions
{
    public IServiceCollection? Services { get; set; }

    public ITypeList<IEventHandler> Handlers { get; }

    public EtoMappingDictionary EtoMappings { get; set; }

    public SkywalkerEventBusOptions()
    {
        Handlers = new TypeList<IEventHandler>();
        EtoMappings = new EtoMappingDictionary();
    }
}
