namespace Skywalker.Ddd.Domain.Entities.Events.Distributed;

[Serializable]
public abstract class Eto
{
    public Dictionary<string, object> Properties { get; }

    protected Eto()
    {
        Properties = new Dictionary<string, object>();
    }
}
