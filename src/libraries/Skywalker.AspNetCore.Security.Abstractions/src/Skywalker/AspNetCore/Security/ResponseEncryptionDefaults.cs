﻿namespace Skywalker.AspNetCore.Security;

public class ResponseEncryptionDefaults
{
    /// <summary>
    /// Default MIME types to compress responses for.
    /// </summary>
    // This list is not intended to be exhaustive, it's a baseline for the 90% case.
    public static readonly IEnumerable<string> MimeTypes = new[]
    {
        // General
        "text/plain",
        // Static files
        "text/css",
        "application/javascript",
        // MVC
        "text/html",
        "application/xml",
        "text/xml",
        "application/json",
        "text/json",
    };
}
