using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Simple.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore;

namespace ActivityCenter.DbMigrator
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().StartAsync();
        }

        internal static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .ConfigureServices((hostContext, services) =>
                       {
                           services.AddHostedService<DbMigratorHostedService>();
                           services.AddSkywalker(skywalker =>
                           {
                               skywalker.AddEntityFrameworkCore<SimpleMigrationsDbContext>(options =>
                               {
                                   options.UseMySql(mysql =>
                                   {
                                       mysql.MigrationsAssembly("Simple.EntityFrameworkCore.DbMigrations");
                                   });
                               });
                           });
                       })
                       .ConfigureAppConfiguration(configure =>
                       {
                           configure.AddEnvironmentVariables();
                           configure.AddCommandLine(args);
                           configure.AddJsonFile("appsettings.json", true, true);
                       });
        }
    }
}
