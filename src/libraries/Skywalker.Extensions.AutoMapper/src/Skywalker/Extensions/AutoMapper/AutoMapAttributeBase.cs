// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using AutoMapper;

namespace Skywalker.Extensions.AutoMapper;

public abstract class AutoMapAttributeBase : Attribute
{
    public Type[] TargetTypes { get; private set; }

    protected AutoMapAttributeBase(params Type[] targetTypes)
    {
        TargetTypes = targetTypes;
    }

    public abstract void CreateMap(IMapperConfigurationExpression configuration, Type type);
}

