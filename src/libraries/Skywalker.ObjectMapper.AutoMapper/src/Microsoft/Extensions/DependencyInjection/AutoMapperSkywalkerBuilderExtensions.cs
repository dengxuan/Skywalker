using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.ObjectMapper;
using Skywalker.ObjectMapper.AutoMapper;

namespace Microsoft.Extensions.DependencyInjection;

public static class AutoMapperSkywalkerBuilderExtensions
{
    public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<SkywalkerAutoMapperOptions> optionsBuilder)
    {
        services.Configure(optionsBuilder);
        services.AddTransient<IObjectMapper, DefaultObjectMapper>();
        services.AddTransient(typeof(IObjectMapper<>), typeof(DefaultObjectMapper<>));
        services.TryAddSingleton<IAutoObjectMappingProvider, AutoMapperAutoObjectMappingProvider>();
        services.TryAddSingleton(typeof(IAutoObjectMappingProvider<>), typeof(AutoMapperAutoObjectMappingProvider<>));

        services.AddSingleton<IMapperAccessor, MapperAccessor>();

        services.TryAddSingleton(SkywalkerAutoMapperOptionsFactory.Create);

        return services;
    }
}
