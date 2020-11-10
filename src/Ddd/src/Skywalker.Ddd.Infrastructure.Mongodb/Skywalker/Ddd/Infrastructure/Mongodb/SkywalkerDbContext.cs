using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Skywalker.Ddd.Infrastructure.Mongodb
{
    public abstract class SkywalkerDbContext : ISkywalkerContext, ITransientDependency
    {
        public IMongoModelSource ModelSource { get; set; }

        public IMongoDatabase Database { get; private set; }

        protected SkywalkerDbContext(IMongoModelSource modelSource)
        {
            ModelSource = modelSource;
        }

        protected internal virtual void CreateModel(IMongoModelBuilder modelBuilder)
        {

        }

        public virtual void InitializeDatabase(IMongoDatabase database)
        {
            Database = database;
        }

        public virtual IMongoCollection<T> Collection<T>()
        {
            return Database.GetCollection<T>(GetCollectionName<T>());
        }

        protected virtual string GetCollectionName<T>()
        {
            return GetEntityModel<T>().CollectionName;
        }

        protected virtual IMongoEntityModel GetEntityModel<TEntity>()
        {
            var model = ModelSource.GetModel(this).Entities.GetOrDefault(typeof(TEntity));

            if (model == null)
            {
                throw new SkywalkerException("Could not find a model for given entity type: " + typeof(TEntity).AssemblyQualifiedName);
            }

            return model;
        }
    }
}