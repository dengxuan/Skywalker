using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Infrastruture.Extensions.EntityFrameworkCore;
using System;

namespace Skywalker.EntityFrameworkCore
{
    public class SkywalkerDbContextProvider<TDbContext> : ISkywalkerDbContextProvider<TDbContext> where TDbContext : DbContext
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
