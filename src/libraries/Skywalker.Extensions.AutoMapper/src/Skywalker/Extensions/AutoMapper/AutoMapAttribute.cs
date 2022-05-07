// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;

namespace Skywalker.Extensions.AutoMapper;

public class AutoMapAttribute : AutoMapAttributeBase
{
    public AutoMapAttribute(params Type[] targetTypes)
        : base(targetTypes)
    {

    }

    public override void CreateMap(IMapperConfigurationExpression configuration, Type type)
    {
        if (TargetTypes.IsNullOrEmpty())
        {
            return;
        }

        foreach (var targetType in TargetTypes)
        {
            configuration.CreateMap(type, targetType, MemberList.Source);
            configuration.CreateMap(targetType, type, MemberList.Destination);
        }
    }
}
