// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.ObjectMapper;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AutoMapFromAttribute : Attribute
{
    public Type[] TargetTypes { get; }

    public AutoMapFromAttribute(params Type[] targetTypes)
    {
        TargetTypes = targetTypes;
    }
}

