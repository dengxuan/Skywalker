using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.Ddd.ObjectMapping.AutoMapper
{
    internal interface IMapperAccessor : ISingletonDependency
    {
        IMapper? Mapper { get; set; }
    }
}
