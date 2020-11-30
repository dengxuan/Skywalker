using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Skywalker.Lightning.Cluster.Hosting
{
    class Program
    {
        static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder().ConfigureServices(services =>
            {
                services.AddLightningCluster();
            }).Build();
        }
    }
}
