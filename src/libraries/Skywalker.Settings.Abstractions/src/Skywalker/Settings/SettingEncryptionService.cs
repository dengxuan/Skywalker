using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Skywalker.Security.Cryptography;
using Skywalker.Settings.Abstractions;

namespace Volo.Abp.Settings;

public class SettingEncryptionService : ISettingEncryptionService//, ITransientDependency
{
    private readonly SettingCryptionOptions _options;

    public SettingEncryptionService(IOptions<SettingCryptionOptions> options)
    {
        _options = options.Value;
    }

    public virtual string? Encrypt(SettingDefinition settingDefinition, string plainValue)
    {
        if (plainValue.IsNullOrEmpty())
        {
            return plainValue;
        }

        return plainValue.ToAes(_options.DefaultPassPhrase,_options.DefaultSalt ,_options.InitVectorBytes,_options.Keysize);
    }

    public virtual string Decrypt(SettingDefinition settingDefinition, string encryptedValue)
    {
        if (encryptedValue.IsNullOrEmpty())
        {
            return encryptedValue;
        }

        try
        {
            return encryptedValue.FromAes(_options.DefaultPassPhrase, _options.DefaultSalt, _options.InitVectorBytes, _options.Keysize);
        }
        catch (Exception e)
        {
            return string.Empty;
        }
    }
}
