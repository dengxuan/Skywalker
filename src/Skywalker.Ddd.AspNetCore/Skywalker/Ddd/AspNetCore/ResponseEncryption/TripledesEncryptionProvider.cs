// Licensed to the Gordon

using System.Security.Cryptography;

namespace Skywalker.Ddd.AspNetCore.ResponseEncryption;

/// <summary>
/// TripleDES encryption provider.
/// </summary>
public class TripledesEncryptionProvider : IEncryptionProvider
{

    /// <inheritdoc />
    public const string Name = "3des";

    public required byte[] Key { get; set; }

    public CipherMode Mode { get; set; }

    public PaddingMode Padding { get; set; }

    /// <inheritdoc />
    public async Task<CryptoStream> CreateStream(Stream outputStream)
    {
        var algorithm = TripleDES.Create();
        algorithm.Mode = Mode;
        algorithm.Padding = Padding;
        algorithm.Key = Key;
        if(Mode != CipherMode.ECB)
        {
            algorithm.GenerateIV();
            await outputStream.WriteAsync(algorithm.IV.AsMemory(0, algorithm.IV.Length));
            await outputStream.FlushAsync();
            Console.WriteLine(algorithm.IV.ToHex());
        }
        var cryptoTransform = algorithm.CreateEncryptor();
        var stream = new CryptoStream(outputStream, cryptoTransform, CryptoStreamMode.Write);
        return stream;
    }
}
