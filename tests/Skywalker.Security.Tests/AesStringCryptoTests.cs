using System.Security.Cryptography;

namespace Skywalker.Security.Tests;

public class AesStringCryptoTests
{
    private static readonly byte[] Salt = new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64 };
    private static readonly byte[] IV = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
    private const string PassPhrase = "TestPassPhrase123";

    [Fact]
    public void Encrypt_ThenDecrypt_ReturnsOriginalText()
    {
        var original = "Hello, World!";

        var encrypted = original.ToAes(PassPhrase, Salt, IV);
        Assert.NotNull(encrypted);
        Assert.NotEqual(original, encrypted);

        var decrypted = encrypted.FromAes(PassPhrase, Salt, IV);
        Assert.Equal(original, decrypted);
    }

    [Fact]
    public void Encrypt_NullInput_ReturnsNull()
    {
        string? nullText = null;

        var result = nullText.ToAes(PassPhrase, Salt, IV);

        Assert.Null(result);
    }

    [Fact]
    public void Decrypt_NullInput_ReturnsNull()
    {
        string? nullText = null;

        var result = nullText.FromAes(PassPhrase, Salt, IV);

        Assert.Null(result);
    }

    [Fact]
    public void Encrypt_EmptyString_ReturnsEncryptedData()
    {
        var encrypted = string.Empty.ToAes(PassPhrase, Salt, IV);

        Assert.NotNull(encrypted);

        var decrypted = encrypted.FromAes(PassPhrase, Salt, IV);
        Assert.Equal(string.Empty, decrypted);
    }

    [Fact]
    public void Encrypt_UnicodeText_RoundTrips()
    {
        var original = "你好世界 🌍 مرحبا";

        var encrypted = original.ToAes(PassPhrase, Salt, IV);
        var decrypted = encrypted!.FromAes(PassPhrase, Salt, IV);

        Assert.Equal(original, decrypted);
    }

    [Fact]
    public void Decrypt_WithWrongPassPhrase_ReturnsGarbageOrThrows()
    {
        var original = "test data";
        var encrypted = original.ToAes(PassPhrase, Salt, IV);

        // With wrong passphrase, either throws CryptographicException
        // or returns garbled text that doesn't match the original
        try
        {
            var decrypted = encrypted!.FromAes("WrongPassPhrase", Salt, IV);
            Assert.NotEqual(original, decrypted);
        }
        catch (CryptographicException)
        {
            // This is acceptable behavior — wrong key causes crypto error
        }
    }

    [Fact]
    public void Encrypt_DifferentPassPhrases_ProduceDifferentOutput()
    {
        var original = "test data";

        var encrypted1 = original.ToAes("Pass1", Salt, IV);
        var encrypted2 = original.ToAes("Pass2", Salt, IV);

        Assert.NotEqual(encrypted1, encrypted2);
    }
}
