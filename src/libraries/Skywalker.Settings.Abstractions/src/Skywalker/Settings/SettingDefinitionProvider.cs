using Skywalker.Settings.Abstractions;

namespace Skywalker.Settings;

public abstract class SettingDefinitionProvider : ISettingDefinitionProvider//, ITransientDependency
{
    public abstract void Define(ISettingDefinitionContext context);
}
