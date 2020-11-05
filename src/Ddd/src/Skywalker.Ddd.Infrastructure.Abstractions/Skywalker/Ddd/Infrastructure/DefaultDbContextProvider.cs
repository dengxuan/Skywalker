using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Infrastructure.Abstractions;
using System;

namespace Skywalker.Ddd.Infrastructure
{
    public class DefaultDbContextProvider<TDbContext> : IDbContextProvider<TDbContext> where TDbContext : SkywalkerDbContext<TDbContext>
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultDbContextProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TDbContext GetDbContext()
        {
            return _serviceProvider.GetRequiredService<TDbContext>();
        }
    }
}
