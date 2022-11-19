// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using AutoMapper;
using Skywalker.Ddd.Application.Abstractions;

namespace Skywalker.Ddd.Application;

/// <summary>
/// 
/// </summary>
public abstract class ApplicationService : IApplicationService
{
    /// <summary>
    /// 
    /// </summary>
    protected IMapper Mapper { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mapper"></param>
    protected ApplicationService (IMapper mapper) => Mapper = mapper;
}
