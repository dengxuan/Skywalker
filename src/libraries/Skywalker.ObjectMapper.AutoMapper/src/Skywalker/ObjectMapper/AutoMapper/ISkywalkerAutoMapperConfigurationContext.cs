using AutoMapper;

namespace Skywalker.ObjectMapper.AutoMapper;

public interface ISkywalkerAutoMapperConfigurationContext
{
    IMapperConfigurationExpression MapperConfiguration { get; }

    IServiceProvider ServiceProvider { get; }
}
