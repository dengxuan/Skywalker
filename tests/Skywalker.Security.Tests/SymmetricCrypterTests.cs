using System.Security.Cryptography;
using Skywalker.Security.Cryptography;

namespace Skywalker.Security.Tests;

public class SymmetricCrypterTests
{
    [Fact]
    public void Encrypt_ProducesNonEmptyOutput()
    {
        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.GenerateKey();
        aes.GenerateIV();

        var crypter = new SymmetricCrypter(aes);
        var original = System.Text.Encoding.UTF8.GetBytes("Hello, World!");

        var encrypted = crypter.Encrypt(original);
        Assert.NotEmpty(encrypted);
        Assert.NotEqual(original, encrypted);
    }

    [Fact]
    public void Encrypt_EmptyInput_ReturnsNonEmptyOutput()
    {
        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.GenerateKey();
        aes.GenerateIV();

        var crypter = new SymmetricCrypter(aes);

        var encrypted = crypter.Encrypt(Array.Empty<byte>());

        Assert.NotEmpty(encrypted);
    }

    [Fact]
    public void CrypterFactory_CreateSymmetricCrypter_AES_ReturnsInstance()
    {
        var factory = new CrypterFactory();
        var crypter = factory.CreateSymmetricCrypter(
            SymmetricCrypterAlgorithms.AES,
            CipherMode.CBC,
            PaddingMode.PKCS7);

        Assert.NotNull(crypter);
    }

    [Fact]
    public void CrypterFactory_CreateSymmetricCrypter_WithKeyAndIV_ReturnsInstance()
    {
        using var aes = Aes.Create();
        aes.GenerateKey();
        aes.GenerateIV();
        var key = Convert.ToBase64String(aes.Key);
        var iv = Convert.ToBase64String(aes.IV);

        var factory = new CrypterFactory();
        var crypter = factory.CreateSymmetricCrypter(
            SymmetricCrypterAlgorithms.AES,
            CipherMode.CBC,
            PaddingMode.PKCS7,
            key,
            iv);

        Assert.NotNull(crypter);
    }

    [Fact]
    public void CrypterFactory_CreateSymmetricCrypter_TripleDES_ReturnsInstance()
    {
        var factory = new CrypterFactory();
        var crypter = factory.CreateSymmetricCrypter(
            SymmetricCrypterAlgorithms.TripleDES,
            CipherMode.CBC,
            PaddingMode.PKCS7);

        Assert.NotNull(crypter);
    }
}
