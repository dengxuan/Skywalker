using Microsoft.Extensions.DependencyInjection;
using System;

namespace Skywalker.EntityFrameworkCore
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
