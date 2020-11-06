using Skywalker.Ddd.Infrastructure.Domain.Repositories;
using Skywalker.Domain.Repositories;
using Skywalker.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public class SkywalkerRepositoryRegistrar : RepositoryRegistrarBase<SkywalkerDbContextRegistrationOptions>
    {
        public SkywalkerRepositoryRegistrar(SkywalkerDbContextRegistrationOptions options) : base(options)
        {

        }

        protected override IEnumerable<Type> GetEntityTypes(Type dbContextType)
        {
            return DbContextHelper.GetEntityTypes(dbContextType);
        }

        protected override Type GetRepositoryType(Type entityType)
        {
            return typeof(SkywalkerRepository<>).MakeGenericType(entityType);
        }

        protected override Type GetRepositoryType(Type entityType, Type primaryKeyType)
        {
            return typeof(SkywalkerRepository<,>).MakeGenericType(entityType, primaryKeyType);
        }
    }
}