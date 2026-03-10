namespace Skywalker.Ddd.Domain.Events.Distributed;

[Serializable]
public abstract class Eto
{
    public Dictionary<string, object> Properties { get; }

    protected Eto()
    {
        Properties = new Dictionary<string, object>();
    }
}
