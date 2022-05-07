// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using AutoMapper;

namespace Skywalker.Extensions.AutoMapper;

internal static class AutoMapperConfigurationExtensions
{
    public static void CreateAutoAttributeMaps(this IMapperConfigurationExpression configuration, Type type)
    {
        foreach (var autoMapAttribute in type.GetCustomAttributes<AutoMapAttributeBase>())
        {
            autoMapAttribute.CreateMap(configuration, type);
        }
    }
}
