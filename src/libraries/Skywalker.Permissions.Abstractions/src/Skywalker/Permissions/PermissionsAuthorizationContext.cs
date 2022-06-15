// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;

namespace Skywalker.Permissions;

public class PermissionsAuthorizationContext
{
    public MethodInfo Method { get; }

    public PermissionsAuthorizationContext(MethodInfo method)
    {
        Method = method;
    }
}
