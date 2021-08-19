using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ActivityCenter.DbMigrator
{
    class DbMigratorHostedService : IHostedService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IServiceProvider _serviceProvider;

        public DbMigratorHostedService(IHostApplicationLifetime hostApplicationLifetime, IServiceProvider serviceProvider)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                //SimpleDbMigrationService dbMigrationService = _serviceProvider.GetRequiredService<SimpleDbMigrationService>();
                //await dbMigrationService.MigrateAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
