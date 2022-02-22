using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Skywalker.Security.Cryptography.Abstractions;

public interface ICrypterFactory
{
    ICrypter CreateSymmetricCrypter(SymmetricCrypterAlgorithms algorithm, CipherMode cipherMode, PaddingMode paddingMode);

    ICrypter CreateSymmetricCrypter(SymmetricCrypterAlgorithms algorithm, CipherMode cipherMode, PaddingMode paddingMode, string key, string iv);

    ICrypter CreateAsymmetricCrypter(AsymmetricCrypterAlgorithms algorithm, CipherMode cipherMode, PaddingMode paddingMode, X509Certificate2 x509Certificate2);
}
