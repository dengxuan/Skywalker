using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Skywalker.Ddd.ObjectMapping.AutoMapper;

internal class SkywalkerAutoMapperOptionsFactory
{
    public static SkywalkerAutoMapperOptions Create(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var options = scope.ServiceProvider.GetRequiredService<IOptions<SkywalkerAutoMapperOptions>>().Value;

        void ConfigureAll(ISkywalkerAutoMapperConfigurationContext ctx)
        {
            foreach (var configurator in options.Configurators)
            {
                configurator(ctx);
            }
        }

        void ValidateAll(IConfigurationProvider config)
        {
            foreach (var profileType in options.ValidatingProfiles)
            {
                config.AssertConfigurationIsValid(profileName: ((Profile)Activator.CreateInstance(profileType)!).ProfileName);
            }
        }

        var mapperConfiguration = new MapperConfiguration(mapperConfigurationExpression =>
        {
            ConfigureAll(new SkywalkerAutoMapperConfigurationContext(mapperConfigurationExpression, scope.ServiceProvider));
        });

        ValidateAll(mapperConfiguration);

        scope.ServiceProvider.GetRequiredService<IMapperAccessor>().Mapper = mapperConfiguration.CreateMapper();
        return options;
    }
}
