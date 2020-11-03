using System.Collections.Generic;

namespace Skywalker.Domain.Entities
{
    //TODO: Re-consider this interface

    public interface IGeneratesDomainEvents
    {

        IEnumerable<object> GetDistributedEvents();

        void ClearDistributedEvents();
    }
}
