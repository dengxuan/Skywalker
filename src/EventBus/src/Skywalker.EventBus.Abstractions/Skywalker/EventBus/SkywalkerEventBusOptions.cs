using Skywalker.Collections;

namespace Skywalker.EventBus
{
    public class SkywalkerEventBusOptions
    {
        public ITypeList<IEventHandler> Handlers { get; }
        public EtoMappingDictionary EtoMappings { get; set; }

        public SkywalkerEventBusOptions()
        {
            Handlers = new TypeList<IEventHandler>();
            EtoMappings = new EtoMappingDictionary();
        }
    }
}