// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Permissions.Abstractions;

public abstract class PermissionDefinitionProvider : IPermissionDefinitionProvider
{
    public virtual void PreDefine(IPermissionDefinitionContext context)
    {

    }

    public abstract void Define(IPermissionDefinitionContext context);

    public virtual void PostDefine(IPermissionDefinitionContext context)
    {

    }
}
