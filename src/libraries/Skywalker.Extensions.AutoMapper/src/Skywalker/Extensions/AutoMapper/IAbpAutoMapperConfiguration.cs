// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using AutoMapper;

namespace Skywalker.Extensions.AutoMapper;

internal interface IAbpAutoMapperConfiguration
{
    List<Action<IMapperConfigurationExpression>> Configurators { get; }
}
