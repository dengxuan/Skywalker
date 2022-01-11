using AutoMapper;

namespace Skywalker.Ddd.ObjectMapping.AutoMapper;

public class SkywalkerAutoMapperConfigurationContext : ISkywalkerAutoMapperConfigurationContext
{
    public IMapperConfigurationExpression MapperConfiguration { get; }
    public IServiceProvider ServiceProvider { get; }

    public SkywalkerAutoMapperConfigurationContext(
        IMapperConfigurationExpression mapperConfigurationExpression,
        IServiceProvider serviceProvider)
    {
        MapperConfiguration = mapperConfigurationExpression;
        ServiceProvider = serviceProvider;
    }
}
