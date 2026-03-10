// Licensed to the Gordon

using System.Security.Cryptography;

namespace Skywalker.Ddd.AspNetCore.ResponseEncryption;

/// <summary>
/// Provides a specific encryption implementation to encrypt HTTP responses.
/// </summary>
public interface IEncryptionProvider
{
    /// <summary>
    /// The key used for encryption.
    /// </summary>
    byte[] Key { get; set; }

    /// <summary>
    /// The cipher mode used for encryption.
    /// </summary>
    CipherMode Mode { get; set; }

    /// <summary>
    /// The padding mode used for encryption.
    /// </summary>
    PaddingMode Padding { get; set; }

    /// <summary>
    /// Create a new encryption stream.
    /// </summary>
    /// <param name="outputStream">The stream where the encrypted data have to be written.</param>
    /// <returns>The encryption stream.</returns>
    Task<CryptoStream> CreateStream(Stream outputStream);
}
