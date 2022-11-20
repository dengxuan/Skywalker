using Volo.Abp.Settings;

namespace Skywalker.Settings.Abstractions;

public interface ISettingEncryptionService
{
    string? Encrypt(SettingDefinition settingDefinition, string? plainValue);

    string? Decrypt(SettingDefinition settingDefinition, string? encryptedValue);
}
