using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.Ddd.EntityFrameworkCore;

//public class DbContextProvider<TDbContext> : IDbContextProvider<TDbContext> where TDbContext : DbContext
//{
//    private readonly IServiceProvider _serviceProvider;

//    public DbContextProvider(IServiceProvider serviceProvider)
//    {
//        _serviceProvider = serviceProvider;
//    }

//    public TDbContext GetDbContext()
//    {
//        return _serviceProvider.GetRequiredService<TDbContext>();
//    }
//}
