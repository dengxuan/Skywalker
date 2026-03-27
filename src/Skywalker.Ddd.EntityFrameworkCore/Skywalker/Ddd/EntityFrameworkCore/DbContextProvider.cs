using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.Ddd.EntityFrameworkCore;

/// <summary>
///
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public class DbContextProvider<TDbContext> : IDbContextProvider<TDbContext> where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///
    /// </summary>
    /// <param name="serviceProvider"></param>
    public DbContextProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public TDbContext GetDbContext()
    {
        var dbContextFactory = _serviceProvider.GetRequiredService<IDbContextFactory<TDbContext>>();
        return dbContextFactory.CreateDbContext();
    }
}
