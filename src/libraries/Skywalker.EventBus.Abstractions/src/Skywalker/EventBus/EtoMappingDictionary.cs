using System;
using System.Collections.Generic;

namespace Skywalker.EventBus;

public class EtoMappingDictionary : Dictionary<Type, EtoMappingDictionaryItem>
{
    public void Add<TEntity, TEntityEto>(Type? objectMappingContextType = null)
    {
        this[typeof(TEntity)] = new EtoMappingDictionaryItem(typeof(TEntityEto), objectMappingContextType);
    }
}
