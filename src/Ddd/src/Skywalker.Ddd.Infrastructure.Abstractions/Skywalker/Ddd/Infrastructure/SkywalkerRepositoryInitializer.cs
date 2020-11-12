using Microsoft.Extensions.DependencyInjection;
using Skywalker.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Skywalker.Ddd.Infrastructure
{
    public class SkywalkerRepositoryInitializer
    {
        internal IServiceCollection Services { get; }

        public SkywalkerRepositoryInitializer(IServiceCollection services)
        {
            Services = services;
        }

        internal void Initialize(Type dbContextType, ISkywalkerDatabaseInitializer databaseInitializer)
        {
            Services.AddDomainServices();
            IEnumerable<Type> entityTypes = DbContextHelper.GetEntityTypes(dbContextType);
            SkywalkerDbContextRegistrationOptions options = new SkywalkerDbContextRegistrationOptions(Services);
            SkywalkerRepositoryRegistrar repositoryRegistrar = new SkywalkerRepositoryRegistrar(options);
            repositoryRegistrar.AddRepositories(entityTypes);
            databaseInitializer.AddDatabases(entityTypes);
        }
    }
}
