using System.Collections.Generic;
using System.Text;

namespace System.Security.Cryptography
{
    public static class HashAlgorithmExtensions
    {
        public static string ToHex(this byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var item in bytes)
            {
                sb.Append(item.ToString("X2"));
            }

            return sb.ToString();
        }

        public static string ToBase64(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static byte[] ToHash(this byte[] bytes, HashAlgorithm algorithm)
        {
            var hashedBytes = algorithm.ComputeHash(bytes);
            return hashedBytes;

        }

        public static bool VerifyHash(this byte[] bytes, byte[] hashedBytes, HashAlgorithm algorithm)
        {
            return bytes.ToHash(algorithm).AreEquivalent(hashedBytes);
        }

        public static byte[] ToMd5(this byte[] bytes)
        {
            using var algorithm = MD5.Create();
            return bytes.ToHash(algorithm);
        }

        public static bool VerifyMd5(this byte[] bytes, byte[] hashedBytes)
        {
            using var algorithm = MD5.Create();
            return bytes.VerifyHash(hashedBytes, algorithm);
        }
    }
}
