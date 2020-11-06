using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Infrastruture.Extensions.EntityFrameworkCore;
using System;

namespace Skywalker.EntityFrameworkCore
{
    public class EntityFrameworkCoreDbContextProvider<TDbContext> : IEntityFrameworkCoreDbContextProvider<TDbContext> where TDbContext : IEntityFrameworkCoreDbContext
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreDbContextProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TDbContext GetDbContext()
        {
            return _serviceProvider.GetRequiredService<TDbContext>();
        }
    }
}
