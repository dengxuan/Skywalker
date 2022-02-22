using Skywalker.Collections.Generic;

namespace Skywalker.Authorization.Permissions;

public class AbpPermissionOptions
{
    public ITypeList<IPermissionDefinitionProvider> DefinitionProviders { get; }

    public ITypeList<IPermissionValueProvider> ValueProviders { get; }

    public AbpPermissionOptions()
    {
        DefinitionProviders = new TypeList<IPermissionDefinitionProvider>();
        ValueProviders = new TypeList<IPermissionValueProvider>();
    }
}
