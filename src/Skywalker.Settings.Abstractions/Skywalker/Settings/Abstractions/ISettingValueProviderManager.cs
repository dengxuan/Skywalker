namespace Skywalker.Settings.Abstractions;

public interface ISettingValueProviderManager
{
    List<ISettingValueProvider> Providers { get; }
}
