using Microsoft.Extensions.DependencyInjection;
using System;

namespace Skywalker.Ddd.Infrastructure.EntityFrameworkCore
{
    public class SkywalkerDbContextProvider<TDbContext> : ISkywalkerDbContextProvider<TDbContext> where TDbContext : SkywalkerDbContext<TDbContext>
    {
        private readonly IServiceProvider _serviceProvider;

        public SkywalkerDbContextProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TDbContext GetDbContext()
        {
            return _serviceProvider.GetRequiredService<TDbContext>();
        }
    }
}
