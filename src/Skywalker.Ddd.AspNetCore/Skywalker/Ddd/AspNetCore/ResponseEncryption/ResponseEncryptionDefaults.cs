// Licensed to the Gordon

namespace Skywalker.Ddd.AspNetCore.ResponseEncryption;

/// <summary>
/// Defaults for the ResponseEncryptionMiddleware
/// </summary>
public class ResponseEncryptionDefaults
{
    /// <summary>
    /// Default MIME types to encrypt responses for.
    /// </summary>
    // This list is not intended to be exhaustive, it's a baseline for the 90% case.
    public static readonly IEnumerable<string> MimeTypes =
    [
            // General
            "text/plain",
            // Static files
            "text/css",
            "application/javascript",
            "text/javascript",
            // MVC
            "text/html",
            "application/xml",
            "text/xml",
            "application/json",
            "text/json",
            // WebAssembly
            "application/wasm",
        ];
}
