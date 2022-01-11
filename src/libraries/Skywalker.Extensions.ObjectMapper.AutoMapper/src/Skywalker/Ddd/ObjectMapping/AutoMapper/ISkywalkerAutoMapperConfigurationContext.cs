using AutoMapper;

namespace Skywalker.Ddd.ObjectMapping.AutoMapper;

public interface ISkywalkerAutoMapperConfigurationContext
{
    IMapperConfigurationExpression MapperConfiguration { get; }

    IServiceProvider ServiceProvider { get; }
}
