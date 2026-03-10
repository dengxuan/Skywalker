using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Skywalker.Ddd.Domain.Entities;

/// <summary>
/// 
/// </summary>
[Serializable]
public abstract class AggregateRoot : Entity, IAggregateRoot, IGeneratesDomainEvents, IHasConcurrencyStamp, IHasCreationTime
{
    private readonly ICollection<object> _distributedEvents = new Collection<object>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventData"></param>
    protected virtual void AddDistributedEvent(object eventData)
    {
        _distributedEvents.Add(eventData);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<object> GetDistributedEvents()
    {
        return _distributedEvents;
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void ClearDistributedEvents()
    {
        _distributedEvents.Clear();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validationErrors = new List<ValidationResult>();
        return validationErrors;
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TKey"></typeparam>
[Serializable]
public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey>, IGeneratesDomainEvents, IHasConcurrencyStamp, IHasCreationTime where TKey : notnull
{

    private readonly ICollection<object> _distributedEvents = new Collection<object>();

    /// <summary>
    /// 
    /// </summary>
    protected AggregateRoot() : this(default!) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    protected AggregateRoot(TKey id) : base(id)
    {
        ConcurrencyStamp = Guid.NewGuid().ToString("N");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventData"></param>
    protected virtual void AddDistributedEvent(object eventData)
    {
        _distributedEvents.Add(eventData);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<object> GetDistributedEvents()
    {
        return _distributedEvents;
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void ClearDistributedEvents()
    {
        _distributedEvents.Clear();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validationErrors = new List<ValidationResult>();
        return validationErrors;
    }
}
