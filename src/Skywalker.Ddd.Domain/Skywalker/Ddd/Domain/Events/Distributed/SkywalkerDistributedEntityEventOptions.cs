namespace Skywalker.Ddd.Domain.Events.Distributed;

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
