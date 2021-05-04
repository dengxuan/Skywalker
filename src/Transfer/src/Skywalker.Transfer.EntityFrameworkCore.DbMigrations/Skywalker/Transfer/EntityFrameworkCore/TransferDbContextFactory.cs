using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywalker.Transfer.EntityFrameworkCore
{
    public class TransferDbContextFactory : IDesignTimeDbContextFactory<TransferMigrationsDbContext>
    {

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }

        public TransferMigrationsDbContext CreateDbContext(string[] args)
        {
            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<TransferMigrationsDbContext>()
                .UseMySql(configuration.GetConnectionString("Transfer"));

            return new TransferMigrationsDbContext(builder.Options);
        }
    }
}
