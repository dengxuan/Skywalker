// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Cryptography;

namespace Skywalker.Ddd.AspNetCore.RequestDecryption;

/// <summary>
/// Decrypts HTTP request bodies using TripleDES.
/// </summary>
/// <remarks>
internal sealed class TripledesDecryptionProvider : IDecryptionProvider
{
    internal const string Name = "3des";

    private const int IVSize = 8;

    public required byte[] Key { get; set; }

    public required CipherMode Mode { get; set; }

    public required PaddingMode Padding { get; set; }

    /// <inheritdoc />
    public async Task<Stream> GetDecryptionStream(Stream stream)
    {
        var algorithm = TripleDES.Create();
        algorithm.Mode = Mode;
        algorithm.Padding = Padding;
        algorithm.Key = Key;
        if (Mode != CipherMode.ECB)
        {
            var iv = new byte[IVSize];
            await stream.ReadExactlyAsync(iv, 0, IVSize);
            algorithm.IV = iv;
        }
        var cryptoTransform = algorithm.CreateDecryptor();
        return new CryptoStream(stream, cryptoTransform, CryptoStreamMode.Read);
    }
}
