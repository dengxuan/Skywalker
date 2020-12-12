using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skywalker.Lightning.Terminal.Abstractions;
using System;

namespace Skywalker.Lighting.Terminal
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddLogging(configure =>
            {
                configure.AddConsole();
            });
            services.AddLighting(builder =>
            {
                builder.AddLightingTerminal(options =>
                {
                }).UseNetty().UseMessagePack();
            });

            IServiceProvider sp = services.BuildServiceProvider();
            ILightningConnector lightningConnector = sp.GetRequiredService<ILightningConnector>();
            lightningConnector.Connect("192.168.1.4", 30000).GetAwaiter().GetResult();
            Console.Read();
            //ILightningTerminalFactory lightningTerminalFactory = sp.GetRequiredService<ILightningTerminalFactory>();
            //ILightningCluster lightningCluster = new LightningCluster("127.0.0.1", 30000, 10, false);
            //ILightningTerminal lightningTerminal = lightningTerminalFactory.CreateTerminalAsync(lightningCluster).GetAwaiter().GetResult();
            //LightningResponse lightningResponse = lightningTerminal.SendAsync(new LightningRequest("", new Dictionary<string, object>())).GetAwaiter().GetResult();
        }
    }
}
