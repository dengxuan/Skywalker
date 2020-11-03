using Skywalker.Extensions.Security.Cryptography.Abstractions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Skywalker.Extensions.Security.Cryptography
{
    public class CrypterFactory : ICrypterFactory
    {
        public ICrypter Create(CrypterAlgorithms algorithm, CipherMode cipherMode, PaddingMode paddingMode)
        {
            switch (algorithm)
            {
                case CrypterAlgorithms.AES: return new SymmetricCrypter(new AesCryptoServiceProvider());
                case CrypterAlgorithms.DES: return new SymmetricCrypter(new DESCryptoServiceProvider());
                case CrypterAlgorithms.TripleDES: return new SymmetricCrypter(new TripleDESCryptoServiceProvider());
                case CrypterAlgorithms.RC2: return new SymmetricCrypter(new RC2CryptoServiceProvider());
                case CrypterAlgorithms.Rijndeal: return new SymmetricCrypter(Rijndael.Create());
                case CrypterAlgorithms.RSA: return new AsymmetricCrypter(new RSACryptoServiceProvider());
                case CrypterAlgorithms.DSA: return new AsymmetricCrypter(new DSACryptoServiceProvider());
                case CrypterAlgorithms.ECDsa: return new AsymmetricCrypter(ECDsa.Create());
                default: throw new ArgumentOutOfRangeException(nameof(algorithm));
            }
        }

        public ICrypter Create(CrypterAlgorithms algorithm, CipherMode cipherMode, PaddingMode paddingMode, string key, string iv)
        {
            throw new NotImplementedException();
        }
    }
}
