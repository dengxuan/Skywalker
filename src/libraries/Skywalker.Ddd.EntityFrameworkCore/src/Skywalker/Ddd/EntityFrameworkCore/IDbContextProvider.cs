using Microsoft.EntityFrameworkCore;
using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Ddd.EntityFrameworkCore;

public interface IDbContextProvider<TDbContext>: ISingletonDependency where TDbContext : DbContext
{
    TDbContext GetDbContext();
}
