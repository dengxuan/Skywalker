namespace Skywalker.Ddd.Domain.Events.Distributed;

[Serializable]
public class EntityEto : Eto
{
    public string EntityType { get; set; }

    public string KeysAsString { get; set; }

    public EntityEto(string entityType, string keysAsString)
    {
        EntityType = entityType;
        KeysAsString = keysAsString;
    }
}
