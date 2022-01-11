using AutoMapper;

namespace Skywalker.ObjectMapper.AutoMapper;

internal class MapperAccessor : IMapperAccessor
{
    public IMapper? Mapper { get; set; }
}
