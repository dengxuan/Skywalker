﻿// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Security.Cryptography.Abstractions;

/// <summary>
/// Can be used to simply encrypt/decrypt texts.
/// Use <see cref="StringCrypterOptions"/> to configure default values.
/// </summary>
public interface IStringCrypter
{
    /// <summary>
    /// Encrypts a text.
    /// </summary>
    /// <param name="plainText">The text in plain format</param>
    /// <param name="passPhrase">A phrase to use as the encryption key (optional, uses default if not provided)</param>
    /// <param name="salt">Salt value (optional, uses default if not provided)</param>
    /// <returns>Enrypted text</returns>
    string? Encrypt(string? plainText, string? passPhrase = null, byte[]? salt = null);

    /// <summary>
    /// Decrypts a text that is encrypted by the <see cref="Encrypt"/> method.
    /// </summary>
    /// <param name="cipherText">The text in encrypted format</param>
    /// <param name="passPhrase">A phrase to use as the encryption key (optional, uses default if not provided)</param>
    /// <param name="salt">Salt value (optional, uses default if not provided)</param>
    /// <returns>Decrypted text</returns>
    string? Decrypt(string? cipherText, string? passPhrase = null, byte[]? salt = null);
}
