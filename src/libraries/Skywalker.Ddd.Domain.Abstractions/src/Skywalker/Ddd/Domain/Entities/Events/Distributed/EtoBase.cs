using System;
using System.Collections.Generic;

namespace Skywalker.Ddd.Domain.Entities.Events.Distributed;

[Serializable]
public abstract class EtoBase
{
    public Dictionary<string, object> Properties { get; }

    protected EtoBase()
    {
        Properties = new Dictionary<string, object>();
    }
}
