using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Skywalker.Aspects;
using System;

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
                .UseInterceptableServiceProvider()
                       .ConfigureWebHostDefaults(webBuilder =>
                       {
                           webBuilder.ConfigureLogging(configureLogging =>
                           {
                               configureLogging.SetMinimumLevel(LogLevel.Trace);
                               configureLogging.AddConsole();
                           });
                           webBuilder.UseStartup<Startup>();
                       });
        }
    }
}
