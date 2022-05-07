// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using AutoMapper;

namespace Skywalker.Extensions.AutoMapper;

public class AutoMapToAttribute : AutoMapAttributeBase
{
    public MemberList MemberList { get; set; } = MemberList.Source;

    public AutoMapToAttribute(params Type[] targetTypes)
        : base(targetTypes)
    {

    }

    public AutoMapToAttribute(MemberList memberList, params Type[] targetTypes)
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
            configuration.CreateMap(type, targetType, MemberList);
        }
    }
}
