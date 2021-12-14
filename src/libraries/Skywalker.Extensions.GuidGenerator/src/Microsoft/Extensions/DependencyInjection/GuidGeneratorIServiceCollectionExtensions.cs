using Skywalker.Extensions.GuidGenerator;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GuidGeneratorIServiceCollectionExtensions
    {
        public static IServiceCollection AddGuidGenerator(this IServiceCollection services)
        {
            services.AddSingleton<IGuidGenerator, SequentialGuidGenerator>();
            return services;
        }
    }
}
