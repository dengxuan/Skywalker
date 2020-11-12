using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Simple.Data;
using Simple.Infrastructure.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Simple.EntityFrameworkCore
{
    public class EntityFrameworkCoreActivityCenterDbSchemaMigrator : ISimpleDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreActivityCenterDbSchemaMigrator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            await _serviceProvider.GetRequiredService<SimpleMigrationsDbContext>()
                                  .Database
                                  .MigrateAsync();
        }
    }
}