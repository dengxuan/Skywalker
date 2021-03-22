using Skywalker.Extensions.Security.Cryptography.Abstractions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Skywalker.Extensions.Security.Cryptography
{
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
                SymmetricCrypterAlgorithms.AES => new SymmetricCrypter(new AesCryptoServiceProvider()),
                SymmetricCrypterAlgorithms.DES => new SymmetricCrypter(new DESCryptoServiceProvider()),
                SymmetricCrypterAlgorithms.TripleDES => new SymmetricCrypter(new TripleDESCryptoServiceProvider()),
                SymmetricCrypterAlgorithms.RC2 => new SymmetricCrypter(new RC2CryptoServiceProvider()),
                SymmetricCrypterAlgorithms.Rijndeal => new SymmetricCrypter(Rijndael.Create()),
                _ => throw new ArgumentOutOfRangeException(nameof(algorithm)),
            };
        }

        public ICrypter CreateSymmetricCrypter(SymmetricCrypterAlgorithms algorithm, CipherMode cipherMode, PaddingMode paddingMode, string key, string iv)
        {
            return algorithm switch
            {
                SymmetricCrypterAlgorithms.AES => new SymmetricCrypter(new AesCryptoServiceProvider()),
                SymmetricCrypterAlgorithms.DES => new SymmetricCrypter(new DESCryptoServiceProvider()),
                SymmetricCrypterAlgorithms.TripleDES => new SymmetricCrypter(new TripleDESCryptoServiceProvider()),
                SymmetricCrypterAlgorithms.RC2 => new SymmetricCrypter(new RC2CryptoServiceProvider()),
                SymmetricCrypterAlgorithms.Rijndeal => new SymmetricCrypter(Rijndael.Create()),
                _ => throw new ArgumentOutOfRangeException(nameof(algorithm)),
            };
        }
    }
}
