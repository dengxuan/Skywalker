using Skywalker.Lightning;
using Skywalker.Lightning.Terminal;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static ILightningBuilder AddLightingTerminal(this ILightningBuilder lightningBuilder, Action<LightningTerminalOptions> options)
        {
            lightningBuilder.Services.Configure(options);
            return lightningBuilder;
        }
    }
}
