using System;
using System.Collections.Generic;
using Skywalker.Ddd.Infrastructure.DbContextConfiguration;
using Skywalker.Domain.Repositories;
using Skywalker.Domain.Repositories.EntityFrameworkCore;

namespace Skywalker.EntityFrameworkCore.DependencyInjection
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

        protected override Type GetRepositoryType(Type dbContextType, Type entityType)
        {
            return typeof(DbContextRepository<,>).MakeGenericType(dbContextType, entityType);
        }

        protected override Type GetRepositoryType(Type dbContextType, Type entityType, Type primaryKeyType)
        {
            return typeof(DbContextRepository<,,>).MakeGenericType(dbContextType, entityType, primaryKeyType);
        }
    }
}