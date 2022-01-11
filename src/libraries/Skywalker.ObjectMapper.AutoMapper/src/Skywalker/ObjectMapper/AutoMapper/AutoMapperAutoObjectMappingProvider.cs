using Skywalker.ObjectMapper;

namespace Skywalker.ObjectMapper.AutoMapper;

internal class AutoMapperAutoObjectMappingProvider<TContext> : AutoMapperAutoObjectMappingProvider, IAutoObjectMappingProvider<TContext>
{
    public AutoMapperAutoObjectMappingProvider(IMapperAccessor mapperAccessor, SkywalkerAutoMapperOptions options)
        : base(mapperAccessor, options)
    {
    }
}

internal class AutoMapperAutoObjectMappingProvider : IAutoObjectMappingProvider
{
    private readonly SkywalkerAutoMapperOptions _options;

    internal IMapperAccessor MapperAccessor { get; }

    public AutoMapperAutoObjectMappingProvider(IMapperAccessor mapperAccessor, SkywalkerAutoMapperOptions options)
    {
        MapperAccessor = mapperAccessor;
        _options = options;
    }

    public virtual TDestination Map<TSource, TDestination>(object source)
    {
        return MapperAccessor.Mapper!.Map<TDestination>(source);
    }

    public virtual TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        return MapperAccessor.Mapper!.Map(source, destination);
    }
}
