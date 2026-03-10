namespace Skywalker.Settings.Abstractions;

public interface ISettingEncryptionService
{
    string? Encrypt(string? plainValue);

    string? Decrypt(string? encryptedValue);
}
