using AutoMapper;

namespace Skywalker.Ddd.ObjectMapping.AutoMapper
{
    internal class MapperAccessor : IMapperAccessor
    {
        public IMapper? Mapper { get; set; }
    }
}