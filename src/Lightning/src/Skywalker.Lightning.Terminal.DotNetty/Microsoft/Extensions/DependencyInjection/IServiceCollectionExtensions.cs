using Skywalker.Lightning;
using Skywalker.Lightning.Terminal.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static ILightningBuilder UseNetty(this ILightningBuilder builder)
        {
            builder.Services.AddSingleton<ILightningTerminal, LightningTerminal>();
            builder.Services.AddSingleton<ILightningTerminalFactory, LightningTerminalFactory>();
            builder.Services.AddSingleton<ILightningConnector, LightningConnector>();
            return builder;
        }
    }
}
