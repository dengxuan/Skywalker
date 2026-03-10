// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Permissions.Abstractions;

public interface IPermissionDefinitionProvider 
{
    void Define(PermissionDefinitionContext context);
}
