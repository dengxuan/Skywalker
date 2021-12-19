using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.Ddd.EntityFrameworkCore;

public class DbContextProvider<TDbContext> : IDbContextProvider<TDbContext> where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;

    public DbContextProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public TDbContext GetDbContext()
    {
#if NETSTANDARD2_0
        return _serviceProvider.GetRequiredService<TDbContext>();
#elif NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP
        IDbContextFactory<TDbContext> dbContextFactory = _serviceProvider.GetRequiredService<IDbContextFactory<TDbContext>>();
        return dbContextFactory.CreateDbContext();
#endif
    }
}
