using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Extensions.Linq;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class EntityFrameworkCoreIServiceCollectionExtensions
{
    public static SkywalkerDbContextBuilder AddEntityFrameworkCore(this IServiceCollection services, Action<SkywalkerDbContextOptions> options)
    {
        services.Configure(options);
        services.TryAddTransient(typeof(IDbContextProvider<>), typeof(DbContextProvider<>));
        services.TryAddTransient<IAsyncQueryableProvider, EfCoreAsyncQueryableProvider>();
        var builder = new SkywalkerDbContextBuilder(services);
        return builder;
    }
}
