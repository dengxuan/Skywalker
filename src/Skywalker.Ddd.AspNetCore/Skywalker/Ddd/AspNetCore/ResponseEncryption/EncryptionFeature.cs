// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Security.Cryptography;

namespace Skywalker.Ddd.AspNetCore.ResponseEncryption;

public class EncryptionFeature(string name, string secureKey, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7) : IEncryptionFeature
{
    public string CryptoName { get; set; } = name;

    public string Key { get; set; } = secureKey;

    public CipherMode Mode { get; set; } = cipherMode;

    public PaddingMode Padding { get; set; } = paddingMode;
}
