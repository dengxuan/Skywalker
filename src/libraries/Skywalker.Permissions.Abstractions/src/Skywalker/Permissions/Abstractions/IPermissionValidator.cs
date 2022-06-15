// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Permissions.Abstractions;

public interface IPermissionValidator : ISingletonDependency
{
    Task<bool> IsGrantedAsync(string name, string providerName, string providerKey);

    Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names, string providerName, string providerKey);
}
