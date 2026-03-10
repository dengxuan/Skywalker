using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Skywalker.Security.Cryptography.Abstractions;

namespace Skywalker.Security.Cryptography;

/// <summary>
/// 非对称加密算法
/// </summary>
public class AsymmetricCrypter : ICrypter
{
    /// <summary>
    /// 非对称加密算法
    /// </summary>
    private readonly AsymmetricAlgorithm _asymmetricAlgorithm;

    private readonly X509Certificate2 _x509Certificate2;

    public AsymmetricCrypter(AsymmetricAlgorithm asymmetricAlgorithm, X509Certificate2 x509Certificate2)
    {
        _asymmetricAlgorithm = asymmetricAlgorithm;
        _x509Certificate2 = x509Certificate2;
    }

    public byte[] Decrypt(byte[] bytes)
    {
        string data = "I'm a programmer!";

        X509Certificate2 prvcrt = new X509Certificate2(@"D:\aaaa.pfx", "cqcca", X509KeyStorageFlags.Exportable);
        if (_x509Certificate2.GetRSAPrivateKey() == null)
        {
            throw new CryptographicException("Private key can't be loaded.");
        }
        var prvkey = _x509Certificate2.GetRSAPrivateKey();
        var pubkey = _x509Certificate2.GetRSAPublicKey();
        try
        {
            using var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(pubkey!.ExportParameters(false));
            rsa.ImportParameters(prvkey!.ExportParameters(true));
            /* 加密 */
            var encryptBytes = rsa.Encrypt(Encoding.UTF8.GetBytes(data), false);
            var encryptString = Convert.ToBase64String(encryptBytes);
            Console.WriteLine("======================加密=============================");
            Console.WriteLine(encryptString);
            Console.WriteLine("======================加密=============================");
            /* 解密 */
            var decryptBytes = rsa.Decrypt(encryptBytes, false);
            var decryptString = Encoding.UTF8.GetString(decryptBytes);
            Console.WriteLine("======================解密=============================");
            Console.WriteLine(decryptString);
            Console.WriteLine("======================解密=============================");
            /* 签名 */
            var signBytes = rsa.SignData(Encoding.UTF8.GetBytes(data), SHA1.Create());
            var signString = Convert.ToBase64String(signBytes);
            Console.WriteLine("======================签名=============================");
            Console.WriteLine(signString);
            Console.WriteLine("======================签名=============================");
        }
        catch (CryptographicException e)
        {
            Console.WriteLine(e.ToString());
        }
        throw new NotImplementedException();
    }

    public byte[] Encrypt(byte[] bytes)
    {
        throw new NotImplementedException();
    }
}
