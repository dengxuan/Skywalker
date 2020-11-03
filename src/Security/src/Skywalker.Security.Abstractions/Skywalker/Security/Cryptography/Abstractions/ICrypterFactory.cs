using System.Security.Cryptography;

namespace Skywalker.Extensions.Security.Cryptography.Abstractions
{
    public interface ICrypterFactory
    {
        ICrypter Create(CrypterAlgorithms algorithm, CipherMode cipherMode, PaddingMode paddingMode);

        ICrypter Create(CrypterAlgorithms algorithm, CipherMode cipherMode, PaddingMode paddingMode, string key, string iv);
    }
}