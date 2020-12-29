using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Skywalker.Aspects;

namespace Simple.WebApi.Hosting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .UseServiceProviderFactory(new AspectsServiceProviderFactory())
                       .ConfigureWebHostDefaults(webBuilder =>
                       {
                           webBuilder.ConfigureLogging(configureLogging =>
                           {
                               configureLogging.AddConsole();
                           });
                           webBuilder.UseStartup<Startup>();
                       });
        }
    }
}
