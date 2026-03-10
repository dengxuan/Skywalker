using System.Text;

namespace System.Security.Cryptography;

public static class HashAlgorithmExtensions
{
    public static string ToHex(this byte[] bytes, bool isUpperCase = true)
    {
        if (bytes == null || bytes.Length == 0)
        {
            throw new ArgumentNullException(nameof(bytes), "Byte array cannot be null or empty.");
        }

        // 初始化 StringBuilder，预估所需容量
        var sb = new StringBuilder(bytes.Length * 2);
        var format = isUpperCase ? "X2" : "x2"; // 大小写控制

        foreach (var item in bytes)
        {
            sb.Append(item.ToString(format));
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
        return bytes.ToHash(algorithm).SequenceEqual(hashedBytes);
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

    public static byte[] ToSha1(this byte[] bytes)
    {
        using var algorithm = SHA1.Create();
        return bytes.ToHash(algorithm);
    }

    public static bool VerifySha1(this byte[] bytes, byte[] hashedBytes)
    {
        using var algorithm = SHA1.Create();
        return bytes.VerifyHash(hashedBytes, algorithm);
    }

    public static byte[] ToSha256(this byte[] bytes)
    {
        using var algorithm = SHA256.Create();
        return bytes.ToHash(algorithm);
    }

    public static bool VerifySha256(this byte[] bytes, byte[] hashedBytes)
    {
        using var algorithm = SHA256.Create();
        return bytes.VerifyHash(hashedBytes, algorithm);
    }

    public static byte[] ToSha384(this byte[] bytes)
    {
        using var algorithm = SHA384.Create();
        return bytes.ToHash(algorithm);
    }

    public static bool VerifySha384(this byte[] bytes, byte[] hashedBytes)
    {
        using var algorithm = SHA384.Create();
        return bytes.VerifyHash(hashedBytes, algorithm);
    }

    public static byte[] ToSha512(this byte[] bytes)
    {
        using var algorithm = SHA512.Create();
        return bytes.ToHash(algorithm);
    }

    public static bool VerifySha512(this byte[] bytes, byte[] hashedBytes)
    {
        using var algorithm = SHA512.Create();
        return bytes.VerifyHash(hashedBytes, algorithm);
    }

    public static byte[] ToHmacMd5(this byte[] bytes)
    {
        using var algorithm = new HMACMD5();
        return bytes.ToHash(algorithm);
    }

    public static byte[] ToHmacMd5(this byte[] bytes, byte[] key)
    {
        using var algorithm = new HMACMD5(key);
        return bytes.ToHash(algorithm);
    }

    public static byte[] ToHmacMd5(this byte[] bytes, string key)
    {
        return bytes.ToHmacMd5(Encoding.UTF8.GetBytes(key));
    }

    public static byte[] ToHmacMd5(this byte[] bytes, string key, Encoding encoding)
    {
        return bytes.ToHmacMd5(encoding.GetBytes(key));
    }

    public static bool VerifyHmacMd5(this byte[] bytes, byte[] hashedBytes)
    {
        using var algorithm = new HMACMD5();
        return bytes.VerifyHash(hashedBytes, algorithm);
    }

    public static bool VerifyHmacMd5(this byte[] bytes, byte[] hashedBytes, byte[] key)
    {
        using var algorithm = new HMACMD5(key);
        return bytes.VerifyHash(hashedBytes, algorithm);
    }

    public static bool VerifyHmacMd5(this byte[] bytes, byte[] hashedBytes, string key)
    {
        return bytes.VerifyHmacMd5(Encoding.UTF8.GetBytes(key), hashedBytes);
    }

    public static bool VerifyHmacMd5(this byte[] bytes, byte[] hashedBytes, string key, Encoding encoding)
    {
        return bytes.VerifyHmacMd5(encoding.GetBytes(key), hashedBytes);
    }

    public static byte[] ToHmacSha1(this byte[] bytes)
    {
        using var algorithm = new HMACSHA1();
        return bytes.ToHash(algorithm);
    }

    public static byte[] ToHmacSha1(this byte[] bytes, byte[] key)
    {
        using var algorithm = new HMACSHA1(key);
        return bytes.ToHash(algorithm);
    }

    public static byte[] ToHmacSha1(this byte[] bytes, string key)
    {
        return bytes.ToHmacSha1(Encoding.UTF8.GetBytes(key));
    }

    public static byte[] ToHmacSha1(this byte[] bytes, string key, Encoding encoding)
    {
        return bytes.ToHmacSha1(encoding.GetBytes(key));
    }

    public static bool VerifyHmacSha1(this byte[] bytes, byte[] hashedBytes)
    {
        using var algorithm = new HMACSHA1();
        return bytes.VerifyHash(hashedBytes, algorithm);
    }

    public static bool VerifyHmacSha1(this byte[] bytes, byte[] hashedBytes, byte[] key)
    {
        using var algorithm = new HMACSHA1(key);
        return bytes.VerifyHash(hashedBytes, algorithm);
    }

    public static bool VerifyHmacSha1(this byte[] bytes, byte[] hashedBytes, string key)
    {
        return bytes.VerifyHmacSha1(Encoding.UTF8.GetBytes(key), hashedBytes);
    }

    public static bool VerifyHmacSha1(this byte[] bytes, byte[] hashedBytes, string key, Encoding encoding)
    {
        return bytes.VerifyHmacSha1(encoding.GetBytes(key), hashedBytes);
    }

    public static byte[] ToHmacSha256(this byte[] bytes)
    {
        using var algorithm = new HMACSHA256();
        return bytes.ToHash(algorithm);
    }

    public static byte[] ToHmacSha256(this byte[] bytes, byte[] key)
    {
        using var algorithm = new HMACSHA256(key);
        return bytes.ToHash(algorithm);
    }

    public static bool VerifyHmacSha256(this byte[] bytes, byte[] hashedBytes)
    {
        using var algorithm = new HMACSHA256();
        return bytes.VerifyHash(hashedBytes, algorithm);
    }

    public static bool VerifyHmacSha256(this byte[] bytes, byte[] hashedBytes, byte[] key)
    {
        using var algorithm = new HMACSHA256(key);
        return bytes.VerifyHash(hashedBytes, algorithm);
    }

    public static bool VerifyHmacSha256(this byte[] bytes, byte[] hashedBytes, string key)
    {
        return bytes.VerifyHmacSha256(hashedBytes, Encoding.UTF8.GetBytes(key));
    }

    public static bool VerifyHmacSha256(this byte[] bytes, byte[] hashedBytes, string key, Encoding encoding)
    {
        return bytes.VerifyHmacSha256(hashedBytes, encoding.GetBytes(key));
    }

    public static byte[] ToHmacSha384(this byte[] bytes)
    {
        using var algorithm = new HMACSHA384();
        return bytes.ToHash(algorithm);
    }

    public static byte[] ToHmacSha384(this byte[] bytes, byte[] key)
    {
        using var algorithm = new HMACSHA384(key);
        return bytes.ToHash(algorithm);
    }

    public static byte[] ToHmacSha384(this byte[] bytes, string key)
    {
        return bytes.ToHmacSha384(Encoding.UTF8.GetBytes(key));
    }

    public static byte[] ToHmacSha384(this byte[] bytes, string key, Encoding encoding)
    {
        return bytes.ToHmacSha384(encoding.GetBytes(key));
    }

    public static bool VerifyHmacSha384(this byte[] bytes, byte[] hashedBytes)
    {
        using var algorithm = new HMACSHA384();
        return bytes.VerifyHash(hashedBytes, algorithm);
    }

    public static bool VerifyHmacSha384(this byte[] bytes, byte[] hashedBytes, byte[] key)
    {
        using var algorithm = new HMACSHA384(key);
        return bytes.VerifyHash(hashedBytes, algorithm);
    }

    public static bool VerifyHmacSha384(this byte[] bytes, byte[] hashedBytes, string key)
    {
        return bytes.VerifyHmacSha384(hashedBytes, Encoding.UTF8.GetBytes(key));
    }

    public static bool VerifyHmacSha384(this byte[] bytes, byte[] hashedBytes, string key, Encoding encoding)
    {
        return bytes.VerifyHmacSha384(hashedBytes, encoding.GetBytes(key));
    }

    public static byte[] ToHmacSha512(this byte[] bytes)
    {
        using var algorithm = new HMACSHA512();
        return bytes.ToHash(algorithm);
    }

    public static byte[] ToHmacSha512(this byte[] bytes, byte[] key)
    {
        using var algorithm = new HMACSHA512(key);
        return bytes.ToHash(algorithm);
    }

    public static byte[] ToHmacSha512(this byte[] bytes, string key)
    {
        return bytes.ToHmacSha512(Encoding.UTF8.GetBytes(key));
    }

    public static byte[] ToHmacSha512(this byte[] bytes, string key, Encoding encoding)
    {
        return bytes.ToHmacSha512(encoding.GetBytes(key));
    }

    public static bool VerifyHmacSha512(this byte[] bytes, byte[] hashedBytes)
    {
        using var algorithm = new HMACSHA512();
        return bytes.VerifyHash(hashedBytes, algorithm);
    }

    public static bool VerifyHmacSha512(this byte[] bytes, byte[] key, byte[] hashedBytes)
    {
        using var algorithm = new HMACSHA512(key);
        return bytes.VerifyHash(hashedBytes, algorithm);
    }

    public static bool VerifyHmacSha512(this byte[] bytes, string key, byte[] hashedBytes)
    {
        return bytes.VerifyHmacSha512(Encoding.UTF8.GetBytes(key), hashedBytes);
    }

    public static bool VerifyHmacSha512(this byte[] bytes, string key, byte[] hashedBytes, Encoding encoding)
    {
        return bytes.VerifyHmacSha512(encoding.GetBytes(key), hashedBytes);
    }
}
