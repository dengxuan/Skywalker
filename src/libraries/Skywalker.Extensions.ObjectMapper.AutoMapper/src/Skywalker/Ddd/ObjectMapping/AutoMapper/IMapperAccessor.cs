using AutoMapper;

namespace Skywalker.Ddd.ObjectMapping.AutoMapper;

internal interface IMapperAccessor
{
    IMapper? Mapper { get; set; }
}
