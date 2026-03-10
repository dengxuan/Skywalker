// Licensed to the Gordon

using System.Security.Cryptography;

namespace Skywalker.Ddd.AspNetCore.RequestDecryption;

/// <summary>
/// Provides a specific decryption implementation to decrypt HTTP request bodies.
/// </summary>
public interface IDecryptionProvider
{
    byte[] Key { get; set; }

    CipherMode Mode { get; set; }

    PaddingMode Padding { get; set; }

    /// <summary>
    /// Creates a new decryption stream.
    /// </summary>
    /// <param name="stream">The encrypted request body stream.</param>
    /// <returns>The decryption stream.</returns>
    Task<Stream> GetDecryptionStream(Stream stream);
}
