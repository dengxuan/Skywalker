// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Permissions.Abstractions;

public interface IPermissionDefinitionProvider:ISingletonDependency
{
    void Define(PermissionDefinitionContext context);
}
