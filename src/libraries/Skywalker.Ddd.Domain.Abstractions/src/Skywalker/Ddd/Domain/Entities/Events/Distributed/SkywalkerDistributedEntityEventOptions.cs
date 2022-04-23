namespace Skywalker.Ddd.Domain.Entities.Events.Distributed;

public class SkywalkerDistributedEntityEventOptions
{
    public IAutoEntityDistributedEventSelectorList AutoEventSelectors { get; }

    public EtoMappingDictionary EtoMappings { get; set; }

    public SkywalkerDistributedEntityEventOptions()
    {
        AutoEventSelectors = new AutoEntityDistributedEventSelectorList();
        EtoMappings = new EtoMappingDictionary();
    }
}
