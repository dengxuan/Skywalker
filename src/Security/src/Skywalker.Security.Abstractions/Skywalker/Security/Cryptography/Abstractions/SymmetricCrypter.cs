using System.IO;
using System.Security.Cryptography;

namespace Skywalker.Extensions.Security.Cryptography.Abstractions
{
    /// <summary>
    /// 对称加密算法加密器
    /// </summary>
    public class SymmetricCrypter : ICrypter
    {
        /// <summary>
        /// 对称加密算法
        /// </summary>
        private readonly SymmetricAlgorithm _symmetricAlgorithm;

        public SymmetricCrypter(SymmetricAlgorithm symmetricAlgorithm)
        {
            _symmetricAlgorithm = symmetricAlgorithm;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="bytes">待解密密文数据流</param>
        /// <returns>解密后的数据流</returns>
        public byte[] Decrypt(byte[] bytes)
        {
            using MemoryStream mStream = new();
            using CryptoStream cStream = new(mStream, _symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Read);
            byte[] fromEncrypt = new byte[bytes.Length];
            //将密文流读入内存流,用指定格式编码为字符串返回
            cStream.Read(fromEncrypt, 0, fromEncrypt.Length);
            return fromEncrypt;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="bytes">待加密数据流</param>
        /// <returns>加密后的密文数据流</returns>
        public byte[] Encrypt(byte[] bytes)
        {
            using MemoryStream mStream = new();
            using CryptoStream cStream = new(mStream, _symmetricAlgorithm.CreateEncryptor(), CryptoStreamMode.Write);
            //将加密的数据流写入内存流
            cStream.Write(bytes, 0, bytes.Length);
            cStream.FlushFinalBlock();

            //从加密后的内存流中获取字节数组
            byte[] ret = mStream.ToArray();
            cStream.Close();
            mStream.Close();
            // 将加密数据转换为Base64字符串返回
            return ret;
        }
    }
}
