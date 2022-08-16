using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Skywalker.Ddd.Domain.Entities;

[Serializable]
public abstract class AggregateRoot : Entity, IAggregateRoot, IGeneratesDomainEvents, IHasConcurrencyStamp, IHasCreationTime
{
    private readonly ICollection<object> _distributedEvents = new Collection<object>();

    protected virtual void AddDistributedEvent(object eventData)
    {
        _distributedEvents.Add(eventData);
    }

    public virtual IEnumerable<object> GetDistributedEvents()
    {
        return _distributedEvents;
    }

    public virtual void ClearDistributedEvents()
    {
        _distributedEvents.Clear();
    }

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validationErrors = new List<ValidationResult>();
        return validationErrors;
    }
}

[Serializable]
public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey>, IGeneratesDomainEvents, IHasConcurrencyStamp, IHasCreationTime where TKey : notnull
{

    private readonly ICollection<object> _distributedEvents = new Collection<object>();

    protected AggregateRoot() : this(default!) { }
    
    protected AggregateRoot(TKey id) : base(id)
    {
        ConcurrencyStamp = Guid.NewGuid().ToString("N");
    }

    protected virtual void AddDistributedEvent(object eventData)
    {
        _distributedEvents.Add(eventData);
    }


    public virtual IEnumerable<object> GetDistributedEvents()
    {
        return _distributedEvents;
    }

    public virtual void ClearDistributedEvents()
    {
        _distributedEvents.Clear();
    }

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validationErrors = new List<ValidationResult>();
        return validationErrors;
    }
}
