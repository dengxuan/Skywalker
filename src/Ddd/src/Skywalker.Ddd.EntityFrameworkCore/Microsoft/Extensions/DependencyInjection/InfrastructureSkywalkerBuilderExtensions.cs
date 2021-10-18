using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;
using Skywalker.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InfrastructureSkywalkerBuilderExtensions
    {
        public static SkywalkerBuilder AddEntityFrameworkCore<TDbContext>(this SkywalkerBuilder skywalker, Action<SkywalkerDbContextOptions> optionsBuilder) where TDbContext : SkywalkerDbContext<TDbContext>
        {
            skywalker.Services.Configure(optionsBuilder);
            skywalker.Services.AddMemoryCache();
            skywalker.Services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
            //skywalker.Services.AddTransient(typeof(IDbContextProvider<>), typeof(DbContextProvider<>));
            skywalker.Services.AddDbContext<TDbContext>();
            skywalker.Services.AddDomainServices();
            SkywalkerDbContextRegistrationOptions options = new(typeof(TDbContext), skywalker.Services);
            SkywalkerRepositoryRegistrar repositoryRegistrar = new(options);
            IEnumerable<Type> entityTypes = DbContextHelper.GetEntityTypes<TDbContext>();
            repositoryRegistrar.AddRepositories(entityTypes);
            return skywalker;
        }
    }
}
