namespace Skywalker.Ddd.Domain.Entities;

//TODO: Re-consider this interface

public interface IGeneratesDomainEvents
{

    IEnumerable<object> GetDistributedEvents();

    void ClearDistributedEvents();
}
