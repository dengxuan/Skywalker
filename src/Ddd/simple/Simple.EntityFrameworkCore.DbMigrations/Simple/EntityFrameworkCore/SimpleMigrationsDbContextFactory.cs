using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Simple.EntityFrameworkCore
{
    /// <summary>
    /// 当前类是EF Core 控制台命令需要的(比如Add-Migration 和 Update-Database 命令)
    /// </summary>
    public class SimpleMigrationsDbContextFactory : IDesignTimeDbContextFactory<SimpleMigrationsDbContext>
    {
        public SimpleMigrationsDbContext CreateDbContext(string[] args)
        {
            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<SimpleMigrationsDbContext>()
                .UseMySql(configuration.GetConnectionString("Simple"), ServerVersion.AutoDetect(configuration.GetConnectionString("Simple")));

            return new SimpleMigrationsDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
