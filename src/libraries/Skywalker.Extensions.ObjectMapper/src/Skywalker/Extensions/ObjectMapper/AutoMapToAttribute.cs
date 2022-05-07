// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.ObjectMapper;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AutoMapToAttribute : Attribute
{
    public Type[] TargetTypes { get; }

    public AutoMapToAttribute(params Type[] targetTypes)
    {
        TargetTypes = targetTypes;
    }
}

