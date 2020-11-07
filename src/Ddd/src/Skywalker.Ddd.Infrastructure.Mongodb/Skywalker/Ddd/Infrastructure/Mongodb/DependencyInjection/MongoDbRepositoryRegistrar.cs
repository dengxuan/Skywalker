using Skywalker.Domain.Repositories;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Repositories.MongoDB;

namespace Volo.Abp.MongoDB.DependencyInjection
{
    public class MongoDbRepositoryRegistrar : RepositoryRegistrarBase<AbpMongoDbContextRegistrationOptions>
    {
        public MongoDbRepositoryRegistrar(AbpMongoDbContextRegistrationOptions options)
            : base(options)
        {

        }

        protected override Type GetRepositoryType(Type entityType)
        {
            return typeof(MongoDbRepository<,>).MakeGenericType(entityType);
        }

        protected override Type GetRepositoryType(Type entityType, Type primaryKeyType)
        {
            return typeof(MongoDbRepository<,,>).MakeGenericType(entityType, primaryKeyType);
        }
    }
}