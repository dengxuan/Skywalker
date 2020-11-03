using Microsoft.Extensions.Hosting;
using Skywalker.Lightning;
using Skywalker.Lightning.Server.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static ILightningBuilder UseNetty(this ILightningBuilder builder)
        {
            builder.Services.AddSingleton<ILightningServer, LightningServer>();
            builder.Services.AddHostedService<LightningServer>();
            return builder;
        }
    }
}
