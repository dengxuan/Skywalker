using AutoMapper;

namespace Skywalker.ObjectMapper.AutoMapper;

internal interface IMapperAccessor
{
    IMapper? Mapper { get; set; }
}
