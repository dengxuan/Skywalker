using AutoMapper;

namespace Skywalker.ObjectMapping.AutoMapper;

/// <summary>
/// AutoMapper implementation of <see cref="IObjectMapper"/>.
/// </summary>
public class AutoMapperObjectMapper : IObjectMapper
{
    private readonly IMapper _mapper;

    /// <summary>
    /// Creates a new AutoMapper object mapper.
    /// </summary>
    /// <param name="mapper">The AutoMapper mapper.</param>
    public AutoMapperObjectMapper(IMapper mapper)
    {
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public TDestination Map<TDestination>(object source)
    {
        return _mapper.Map<TDestination>(source);
    }

    /// <inheritdoc/>
    public TDestination Map<TSource, TDestination>(TSource source)
    {
        return _mapper.Map<TSource, TDestination>(source);
    }

    /// <inheritdoc/>
    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        return _mapper.Map(source, destination);
    }
}

