using Microsoft.Extensions.Hosting;
using Skywalker.Lightning;
using Skywalker.Lightning.Abstractions;
using Skywalker.Lightning.Server;
using Skywalker.Lightning.Server.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static ILightningBuilder AddLightingServer(this ILightningBuilder lightningBuilder, Action<LightningServerOptions> options)
        {
            lightningBuilder.Services.Configure(options);
            lightningBuilder.Services.AddSingleton<ILightningServiceFactory, LightningServiceFactory>();
            return lightningBuilder;
        }
    }
}
