using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skywalker.Lightning.Cluster;
using Skywalker.Lightning.Cluster.Abstractions;
using Skywalker.Lightning.Messaging;
using Skywalker.Lightning.Terminal.Abstractions;
using System;
using System.Collections.Generic;

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
            //IServiceProvider sp = services.BuildServiceProvider();
            //ILightningTerminalFactory lightningTerminalFactory = sp.GetRequiredService<ILightningTerminalFactory>();
            //ILightningCluster lightningCluster = new LightningCluster("127.0.0.1", 30000, 10, false);
            //ILightningTerminal lightningTerminal = lightningTerminalFactory.CreateTerminalAsync(lightningCluster).GetAwaiter().GetResult();
            //LightningResponse lightningResponse = lightningTerminal.SendAsync(new LightningRequest("", new Dictionary<string, object>())).GetAwaiter().GetResult();
        }
    }
}
