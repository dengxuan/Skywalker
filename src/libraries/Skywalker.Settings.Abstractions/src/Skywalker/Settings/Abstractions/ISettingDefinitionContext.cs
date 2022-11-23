using System.Collections.Generic;

namespace Skywalker.Settings.Abstractions;

public interface ISettingDefinitionContext
{
    SettingDefinition GetOrNull(string name);

    IReadOnlyList<SettingDefinition> GetAll();

    void Add(params SettingDefinition[] definitions);
}
