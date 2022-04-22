using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Skywalker.Security.Cryptography.Abstractions;

namespace Skywalker.Security.Cryptography;

public class CrypterFactory : ICrypterFactory
{
    public ICrypter CreateAsymmetricCrypter(AsymmetricCrypterAlgorithms algorithm, CipherMode cipherMode, PaddingMode paddingMode, X509Certificate2 x509Certificate2)
    {
        return algorithm switch
        {
            AsymmetricCrypterAlgorithms.RSA => new AsymmetricCrypter(new RSACryptoServiceProvider(), x509Certificate2),
            AsymmetricCrypterAlgorithms.DSA => new AsymmetricCrypter(new DSACryptoServiceProvider(), x509Certificate2),
            AsymmetricCrypterAlgorithms.ECDsa => new AsymmetricCrypter(ECDsa.Create(), x509Certificate2),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm)),
        };
    }

    public ICrypter CreateSymmetricCrypter(SymmetricCrypterAlgorithms algorithm, CipherMode cipherMode, PaddingMode paddingMode)
    {
        return algorithm switch
        {
            SymmetricCrypterAlgorithms.AES => new SymmetricCrypter(Aes.Create()),
            SymmetricCrypterAlgorithms.DES => new SymmetricCrypter(DES.Create()),
            SymmetricCrypterAlgorithms.TripleDES => new SymmetricCrypter(TripleDES.Create()),
            SymmetricCrypterAlgorithms.RC2 => new SymmetricCrypter(RC2.Create()),
            SymmetricCrypterAlgorithms.Rijndeal => new SymmetricCrypter(Aes.Create("Rijndeal")!),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm)),
        };
    }

    public ICrypter CreateSymmetricCrypter(SymmetricCrypterAlgorithms algorithm, CipherMode cipherMode, PaddingMode paddingMode, string key, string iv)
    {
        return algorithm switch
        {
            SymmetricCrypterAlgorithms.AES => new SymmetricCrypter(Aes.Create()),
            SymmetricCrypterAlgorithms.DES => new SymmetricCrypter(DES.Create()),
            SymmetricCrypterAlgorithms.TripleDES => new SymmetricCrypter(TripleDES.Create()),
            SymmetricCrypterAlgorithms.RC2 => new SymmetricCrypter(RC2.Create()),
            SymmetricCrypterAlgorithms.Rijndeal => new SymmetricCrypter(Aes.Create("Rijndeal")!),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm)),
        };
    }
}
