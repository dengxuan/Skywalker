using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker;
using Skywalker.Ddd.Infrastructure;
using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.Ddd.Infrastruture.Extensions.EntityFrameworkCore;
using Skywalker.EntityFrameworkCore;
using Skywalker.EntityFrameworkCore.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SkywalkerEfCoreServiceCollectionExtensions
    {
        public static SkywalkerDbContextOptions AddEntityFrameworkCore<TDbContext>(this SkywalkerDbContextOptions options)
        {
            //options.Services.Replace(ServiceDescriptor.Singleton<ISkywalkerDbContextAdapter<TDbContext>, EntityFrameworkCoreDbContextAdapter<TDbContext>>());
            //options.Services.AddMemoryCache();
            //options.Services.TryAddTransient(DbContextOptionsFactory.Create<TDbContext>);

            //options.Services.TryAddTransient(typeof(ISkywalkerDbContextProvider<>), typeof(EntityFrameworkCoreDbContextProvider<>));
            //options.Services.AddDbContext<TDbContext>();

            return options;
        }
    }
}
