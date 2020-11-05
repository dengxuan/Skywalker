using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker;
using Skywalker.Ddd.Infrastructure;
using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.EntityFrameworkCore;
using Skywalker.EntityFrameworkCore.DependencyInjection;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SkywalkerEfCoreServiceCollectionExtensions
    {
        //public static SkywalkerBuilder AddEntityFrameworkCore<TDbContext>(this SkywalkerBuilder builder, Action<SkywalkerDbContextOptions> optionsBuilder) where TDbContext : EntityFrameworkCoreDbContext<TDbContext>
        //{
        //    builder.Services.Configure(optionsBuilder);

        //    builder.Services.AddMemoryCache();
        //    var options = new SkywalkerDbContextRegistrationOptions(typeof(TDbContext), builder.Services);
        //    options.AddDefaultRepositories();
        //    builder.Services.TryAddTransient(DbContextOptionsFactory.Create<TDbContext>);

        //    foreach (var dbContextType in options.ReplacedDbContextTypes)
        //    {
        //        builder.Services.Replace(ServiceDescriptor.Transient(dbContextType, typeof(TDbContext)));
        //    }

        //    new EfCoreRepositoryRegistrar(options).AddRepositories();

        //    builder.Services.TryAddTransient(typeof(IDbContextProvider<>), typeof(EntityFrameworkCoreDbContextProvider<>));
        //    builder.Services.AddDbContext<TDbContext>();

        //    return builder;
        //}
    }
}
