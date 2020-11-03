using System;
using System.Collections.Generic;

namespace Skywalker.Domain.Entities.Events.Distributed
{
    public interface IAutoEntityDistributedEventSelectorList : IList<NamedTypeSelector>
    {
    }
}