// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.Collections.Generic;
using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions;

public class PermissionOptions
{
    public ITypeList<IPermissionDefinitionProvider> DefinitionProviders { get; }

    public ITypeList<IPermissionValueProvider> ValueProviders { get; }

    public PermissionOptions()
    {
        DefinitionProviders = new TypeList<IPermissionDefinitionProvider>();
        ValueProviders = new TypeList<IPermissionValueProvider>();
    }
}
