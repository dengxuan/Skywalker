﻿namespace Skywalker.Ddd.Domain.Entities.Events.Distributed;

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
