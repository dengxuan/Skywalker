using System.Collections.Generic;
using Volo.Abp.Settings;

namespace Skywalker.Settings.Abstractions;

public interface ISettingDefinitionContext
{
    SettingDefinition GetOrNull(string name);

    IReadOnlyList<SettingDefinition> GetAll();

    void Add(params SettingDefinition[] definitions);
}
