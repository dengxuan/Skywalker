// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Permissions.Abstractions;

public interface IPermissionValueProvider : ITransientDependency
{
    string Name { get; }

    //TODO: Rename to GetResult? (CheckAsync throws exception by naming convention)
    Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context);

    Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context);
}
