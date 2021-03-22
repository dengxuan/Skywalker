using System.Security.Cryptography;

namespace Skywalker.Security.Cryptography
{
    public struct Crypter
    {
        public int Algorithm { get; set; }

        public byte[]? Key { get; set; }

        public byte[]? IV { get; set; }

        public Crypter(int algorithm)
        {
            Algorithm = (0x01000000 & (int)SymmetricCrypterAlgorithms.AES << 16) & ((int)CipherMode.CBC << 8) & ((int)PaddingMode.PKCS7);
            Key = null;
            IV = null;
        }
    }
}