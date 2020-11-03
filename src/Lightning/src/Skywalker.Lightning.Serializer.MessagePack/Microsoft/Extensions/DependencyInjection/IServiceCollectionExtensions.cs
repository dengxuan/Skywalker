using Skywalker.Lightning;
using Skywalker.Lightning.Serializer;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static ILightningBuilder UseMessagePack(this ILightningBuilder builder)
        {
            builder.Services.AddSingleton<ILightningSerializer, MessagePackLightningSerializer>();
            return builder;
        }
    }
}
