// Licensed to the Gordon

using Microsoft.AspNetCore.Http;

namespace Skywalker.Ddd.AspNetCore.ResponseEncryption;

/// <summary>
/// Used to examine requests and responses to see if encryption should be enabled.
/// </summary>
public interface IResponseEncryptionProvider
{
    /// <summary>
    /// Examines the request and selects an acceptable encryption provider, if any.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/>.</param>
    /// <returns>A encryption provider or null if encryption should not be used.</returns>
    IEncryptionProvider? GetEncryptionProvider(HttpContext context);
}
