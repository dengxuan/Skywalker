using Skywalker.PermissionsEvaluator.Abstractions;
using Skywalker.Extensions.Collections.Generic;

namespace Skywalker.PermissionsEvaluator;

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
