using Skywalker.Security.Cryptography.Abstractions;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Skywalker.Security.Cryptography
{
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
            if (_x509Certificate2.PrivateKey == null)
            {
                throw new CryptographicException("Private key can't be loaded.");
            }
            RSACryptoServiceProvider prvkey = (RSACryptoServiceProvider)_x509Certificate2.PrivateKey;
            RSACryptoServiceProvider pubkey = (RSACryptoServiceProvider)_x509Certificate2.PublicKey.Key;
            try
            {
                using RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(pubkey.ExportParameters(false));
                rsa.ImportParameters(prvkey.ExportParameters(true));
                /* 加密 */
                byte[] encryptBytes = rsa.Encrypt(Encoding.UTF8.GetBytes(data), false);
                string encryptString = Convert.ToBase64String(encryptBytes);
                Console.WriteLine("======================加密=============================");
                Console.WriteLine(encryptString);
                Console.WriteLine("======================加密=============================");
                /* 解密 */
                byte[] decryptBytes = rsa.Decrypt(encryptBytes, false);
                string decryptString = Encoding.UTF8.GetString(decryptBytes);
                Console.WriteLine("======================解密=============================");
                Console.WriteLine(decryptString);
                Console.WriteLine("======================解密=============================");
                /* 签名 */
                byte[] signBytes = rsa.SignData(Encoding.UTF8.GetBytes(data), new SHA1CryptoServiceProvider());
                string signString = Convert.ToBase64String(signBytes);
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
}
