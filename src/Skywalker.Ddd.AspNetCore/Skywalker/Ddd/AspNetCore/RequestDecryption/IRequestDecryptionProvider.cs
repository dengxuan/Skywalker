// Licensed to the Gordon

using Microsoft.AspNetCore.Http;

namespace Skywalker.Ddd.AspNetCore.RequestDecryption;

/// <summary>
/// Used to examine requests to see if decryption should be used.
/// </summary>
public interface IRequestDecryptionProvider
{
    /// <summary>
    /// Examines the request and selects an acceptable decryption provider, if any.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/>.</param>
    /// <returns>The decryption stream when the provider is capable of decrypting the HTTP request body, otherwise <see langword="null" />.</returns>
    Task<Stream?> GetDecryptionStream(HttpContext context);
}
