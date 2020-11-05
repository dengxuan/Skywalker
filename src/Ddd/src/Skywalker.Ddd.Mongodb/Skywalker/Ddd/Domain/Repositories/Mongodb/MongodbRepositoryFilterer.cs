using MongoDB.Driver;
using Skywalker.Data;
using Skywalker.Data.Filtering;
using Skywalker.Domain.Entities;
using System.Collections.Generic;

namespace Volo.Abp.Domain.Repositories.MongoDB
{
    public class MongoDbRepositoryFilterer<TEntity> : IMongoDbRepositoryFilterer<TEntity>
        where TEntity : class, IEntity
    {
        protected IDataFilter DataFilter { get; }

        public MongoDbRepositoryFilterer(IDataFilter dataFilter)
        {
            DataFilter = dataFilter;
        }

        public virtual void AddGlobalFilters(List<FilterDefinition<TEntity>> filters)
        {
            if (typeof(IDeleteable).IsAssignableFrom(typeof(TEntity)) && DataFilter.IsEnabled<IDeleteable>())
            {
                filters.Add(Builders<TEntity>.Filter.Eq(e => ((IDeleteable) e).IsDeleted, false));
            }
        }
    }

    public class MongoDbRepositoryFilterer<TEntity, TKey> : MongoDbRepositoryFilterer<TEntity>,
        IMongoDbRepositoryFilterer<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        public MongoDbRepositoryFilterer(IDataFilter dataFilter)
            : base(dataFilter)
        {
        }

        public FilterDefinition<TEntity> CreateEntityFilter(TKey id, bool applyFilters = false)
        {
            var filters = new List<FilterDefinition<TEntity>>
            {
                Builders<TEntity>.Filter.Eq(e => e.Id, id)
            };

            if (applyFilters)
            {
                AddGlobalFilters(filters);
            }

            return Builders<TEntity>.Filter.And(filters);
        }

        public FilterDefinition<TEntity> CreateEntityFilter(TEntity entity, bool withConcurrencyStamp = false, string concurrencyStamp = null)
        {
            if (!withConcurrencyStamp || !(entity is IHasConcurrencyStamp entityWithConcurrencyStamp))
            {
                return Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);
            }

            if (concurrencyStamp == null)
            {
                concurrencyStamp = entityWithConcurrencyStamp.ConcurrencyStamp;
            }

            return Builders<TEntity>.Filter.And(
                Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id),
                Builders<TEntity>.Filter.Eq(e => ((IHasConcurrencyStamp) e).ConcurrencyStamp, concurrencyStamp)
            );
        }
    }
}