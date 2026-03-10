namespace Skywalker.Settings.Abstractions;

public interface ISettingDefinitionProvider
{
    void Define(ISettingDefinitionContext context);
}
