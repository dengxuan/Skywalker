using Skywalker.Domain.Entities;
using System.Linq;

namespace Skywalker.Ddd.Infrastructure.Abstractions
{
    public interface IDataCollection<TEntity> where TEntity : IEntity
    {
        IQueryable<TEntity> Entities { get; set; }
    }
}
