// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using AutoMapper;
using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Extensions.AutoMapper;

public class AutoMapperObjectMapper : IObjectMapper, ISingletonDependency
{
    private readonly IMapper _mapper;

    public AutoMapperObjectMapper(IMapper mapper)
    {
        _mapper = mapper;
    }

    public TDestination Map<TDestination>(object source)
    {
        return _mapper.Map<TDestination>(source);
    }

    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        return _mapper.Map(source, destination);
    }
}
