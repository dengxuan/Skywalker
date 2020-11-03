using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker;
using Skywalker.EntityFrameworkCore;
using Skywalker.EntityFrameworkCore.DependencyInjection;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SkywalkerEfCoreServiceCollectionExtensions
    {
        public static SkywalkerBuilder AddEntityFrameworkCore<TDbContext>(this SkywalkerBuilder builder, Action<SkywalkerDbContextOptions> optionsBuilder) where TDbContext : SkywalkerDbContext<TDbContext>
        {
            builder.Services.Configure(optionsBuilder);

            builder.Services.AddMemoryCache();
            var options = new SkywalkerDbContextRegistrationOptions(typeof(TDbContext), builder.Services);
            options.AddDefaultRepositories();
            builder.Services.TryAddTransient(DbContextOptionsFactory.Create<TDbContext>);

            foreach (var dbContextType in options.ReplacedDbContextTypes)
            {
                builder.Services.Replace(ServiceDescriptor.Transient(dbContextType, typeof(TDbContext)));
            }

            new EfCoreRepositoryRegistrar(options).AddRepositories();

            builder.Services.TryAddTransient(typeof(IDbContextProvider<>), typeof(DefaultDbContextProvider<>));
            builder.Services.AddDbContext<TDbContext>(b => 
            {
                b.UseLazyLoadingProxies();
            });
            //builder.Services.AddDbContextPool<TDbContext>(options =>
            //{
            //    options.UseLazyLoadingProxies();
            //});

            return builder;
        }
    }
}
