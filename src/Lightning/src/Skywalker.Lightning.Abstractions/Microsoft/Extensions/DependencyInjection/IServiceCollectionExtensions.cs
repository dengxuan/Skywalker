using Skywalker.Lightning;
using Skywalker.Lightning.Abstractions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static ILightningBuilder AddLighting(this IServiceCollection services, Action<ILightningBuilder> lightner)
        {
            services.AddSingleton<ILightningServiceNameGenerator, LightningServiceNameGenerator>();
            LigntingBuilder lignting = new LigntingBuilder(services);
            lightner?.Invoke(lignting);
            return lignting;
        }
    }
}
