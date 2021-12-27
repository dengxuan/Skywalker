using System;
using System.Collections.Generic;

namespace Skywalker.Ddd.Domain.Entities.Events.Distributed
{
    public interface IAutoEntityDistributedEventSelectorList : IList<NamedTypeSelector>
    {
    }
}