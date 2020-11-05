using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Infrastructure.Abstractions;
using System;

namespace Skywalker.EntityFrameworkCore
{
    public class EntityFrameworkCoreDbContextProvider<TDbContext> : IDbContextProvider<TDbContext> where TDbContext : EntityFrameworkCoreDbContext<TDbContext>
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
