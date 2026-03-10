using Skywalker.Settings.Abstractions;

namespace Skywalker.Settings;

public abstract class SettingDefinitionProvider : ISettingDefinitionProvider
{
    public abstract void Define(ISettingDefinitionContext context);
}
