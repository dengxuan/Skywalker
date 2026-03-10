using Skywalker.Extensions.Collections;

namespace Skywalker.Ddd.Domain.Events.Distributed;

public interface IAutoEntityDistributedEventSelectorList : IList<NamedTypeSelector>
{
}
