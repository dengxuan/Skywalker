namespace Skywalker.Settings.Abstractions;

public interface ISettingDefinitionManager
{
    SettingDefinition Get(string name);

    IReadOnlyList<SettingDefinition> GetAll();

    SettingDefinition GetOrNull(string name);
}
