// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Text;

namespace System.Security.Cryptography;

/// <summary>
/// 
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="plainText"></param>
    /// <param name="passPhrase"></param>
    /// <param name="salt"></param>
    /// <param name="initVectorBytes"></param>
    /// <param name="keySize"></param>
    /// <returns></returns>
    public static string? ToAes(this string? plainText, string passPhrase, byte[] salt, byte[] initVectorBytes, int keySize = 256)
    {
        if (plainText == null)
        {
            return null;
        }

        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        using var password = new Rfc2898DeriveBytes(passPhrase, salt);

        var keyBytes = password.GetBytes(keySize / 8);

        using var symmetricKey = Aes.Create();

        symmetricKey.Mode = CipherMode.CBC;

        using var encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
        cryptoStream.FlushFinalBlock();
        var cipherTextBytes = memoryStream.ToArray();
        return Convert.ToBase64String(cipherTextBytes);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cipherText"></param>
    /// <param name="passPhrase"></param>
    /// <param name="salt"></param>
    /// <param name="initVectorBytes"></param>
    /// <param name="keySize"></param>
    /// <returns></returns>
    public static string? FromAes(this string? cipherText, string passPhrase, byte[] salt, byte[] initVectorBytes, int keySize = 256)
    {
        if (string.IsNullOrEmpty(cipherText))
        {
            return null;
        }

        var cipherTextBytes = Convert.FromBase64String(cipherText);
        using var password = new Rfc2898DeriveBytes(passPhrase, salt);

        var keyBytes = password.GetBytes(keySize / 8);
        using var symmetricKey = Aes.Create();

        symmetricKey.Mode = CipherMode.CBC;
        using var decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

        using var memoryStream = new MemoryStream(cipherTextBytes);

        using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

        var plainTextBytes = new byte[cipherTextBytes.Length];
        var totalReadCount = 0;
        while (totalReadCount < cipherTextBytes.Length)
        {
            var buffer = new byte[cipherTextBytes.Length];
            var readCount = cryptoStream.Read(buffer, 0, buffer.Length);
            if (readCount == 0)
            {
                break;
            }

            for (var i = 0; i < readCount; i++)
            {
                plainTextBytes[i + totalReadCount] = buffer[i];
            }

            totalReadCount += readCount;
        }

        return Encoding.UTF8.GetString(plainTextBytes, 0, totalReadCount);
    }
}
