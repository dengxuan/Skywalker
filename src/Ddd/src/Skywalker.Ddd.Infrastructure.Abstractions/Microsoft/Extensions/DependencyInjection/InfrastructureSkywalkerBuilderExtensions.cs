using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker;
using Skywalker.Ddd.Infrastructure;
using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.Ddd.Infrastructure.DbContextConfiguration;
using Skywalker.EntityFrameworkCore.DependencyInjection;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InfrastructureSkywalkerBuilderExtensions
    {
        public static SkywalkerBuilder AddEntityFrameworkCore<TDbContext>(this SkywalkerBuilder builder, Action<SkywalkerDbContextOptions> optionsBuilder) where TDbContext : SkywalkerDbContext<TDbContext>, IDbContext
        {
            builder.Services.Configure(optionsBuilder);

            var options = new SkywalkerDbContextRegistrationOptions(typeof(TDbContext), builder.Services);
            options.AddDefaultRepositories();
            //builder.Services.TryAddTransient(DbContextOptionsFactory.Create<TDbContext>);

            foreach (var dbContextType in options.ReplacedDbContextTypes)
            {
                builder.Services.Replace(ServiceDescriptor.Transient(dbContextType, typeof(TDbContext)));
            }

            new SkywalkerRepositoryRegistrar(options).AddRepositories();

            builder.Services.TryAddTransient(typeof(IDbContextProvider<>), typeof(DefaultDbContextProvider<>));

            return builder;
        }
    }
}
