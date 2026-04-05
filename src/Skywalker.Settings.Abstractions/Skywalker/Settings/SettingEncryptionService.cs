using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Skywalker.Settings.Abstractions;

namespace Skywalker.Settings;

public class SettingEncryptionService(IOptions<SettingCryptionOptions> options) : ISettingEncryptionService
{
    private readonly SettingCryptionOptions _options = options.Value;

    public virtual string? Encrypt(string? plainValue)
    {
        if (plainValue.IsNullOrEmpty())
        {
            return plainValue;
        }

        return plainValue.ToAes(_options.DefaultPassPhrase, _options.DefaultSalt, _options.InitVectorBytes, _options.Keysize);
    }

    public virtual string? Decrypt(string? encryptedValue)
    {
        if (encryptedValue.IsNullOrEmpty())
        {
            return encryptedValue;
        }

        try
        {
            return encryptedValue.FromAes(_options.DefaultPassPhrase, _options.DefaultSalt, _options.InitVectorBytes, _options.Keysize);
        }
        catch
        {
            return null;
        }
    }
}
