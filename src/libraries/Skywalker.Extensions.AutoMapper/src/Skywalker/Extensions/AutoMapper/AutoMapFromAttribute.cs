// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using AutoMapper;

namespace Skywalker.Extensions.AutoMapper;

public class AutoMapFromAttribute : AutoMapAttributeBase
{
    public MemberList MemberList { get; set; } = MemberList.Destination;

    public AutoMapFromAttribute(params Type[] targetTypes)
        : base(targetTypes)
    {

    }

    public AutoMapFromAttribute(MemberList memberList, params Type[] targetTypes)
        : this(targetTypes)
    {
        MemberList = memberList;
    }

    public override void CreateMap(IMapperConfigurationExpression configuration, Type type)
    {
        if (TargetTypes.IsNullOrEmpty())
        {
            return;
        }

        foreach (var targetType in TargetTypes)
        {
            configuration.CreateMap(targetType, type, MemberList.Destination);
        }
    }
}
