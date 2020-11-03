using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Skywalker.Lightning;
using Skywalker.Lightning.Server.Abstractions;
using System;
using System.Threading.Tasks;

namespace Skywalker.Lighting.Server
{
    class Program
    {
        public static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddLogging(configure=>
            {
                configure.AddConsole();
            });
            services.AddLighting(builder =>
            {
                builder.AddLightingServer(options =>
                {
                }).UseNetty().UseMessagePack();
            });
            IServiceProvider sp = services.BuildServiceProvider();
            var hostedService = sp.GetServices<IHostedService>();
            foreach (var item in hostedService)
            {
                item.StartAsync(default).GetAwaiter().GetResult();
            }
        }
    }
}
