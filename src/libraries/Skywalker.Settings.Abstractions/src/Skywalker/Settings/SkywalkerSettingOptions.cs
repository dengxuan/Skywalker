using Skywalker.Extensions.Collections.Generic;
using Skywalker.Settings.Abstractions;

namespace Skywalker.Settings;

public class SkywalkerSettingOptions
{
    public ITypeList<ISettingDefinitionProvider> DefinitionProviders { get; }

    public ITypeList<ISettingValueProvider> ValueProviders { get; }

    public SkywalkerSettingOptions()
    {
        DefinitionProviders = new TypeList<ISettingDefinitionProvider>();
        ValueProviders = new TypeList<ISettingValueProvider>();
    }
}
