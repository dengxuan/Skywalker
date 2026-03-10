// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Security.Cryptography;

namespace Skywalker.Ddd.AspNetCore.ResponseEncryption;

public interface IEncryptionFeature
{
    string CryptoName { get; }

    string Key { get; }

    CipherMode Mode { get; set; }

    PaddingMode Padding { get; set; }

}
