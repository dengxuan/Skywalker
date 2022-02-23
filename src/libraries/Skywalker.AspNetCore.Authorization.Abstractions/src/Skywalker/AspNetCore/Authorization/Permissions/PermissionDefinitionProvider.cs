﻿namespace Skywalker.AspNetCore.Authorization.Permissions;

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
