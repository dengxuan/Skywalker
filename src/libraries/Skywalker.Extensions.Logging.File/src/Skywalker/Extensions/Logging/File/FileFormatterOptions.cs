// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Skywalker.Extensions.Logging.File;

/// <summary>
/// Options for the built-in console log formatter.
/// </summary>
public class FileFormatterOptions
{
    public FileFormatterOptions() { }

    /// <summary>
    /// Includes scopes when <see langword="true" />.
    /// </summary>
    public bool IncludeScopes { get; set; }

    /// <summary>
    /// Gets or sets format string used to format timestamp in logging messages. Defaults to <c>null</c>.
    /// </summary>
    public string? TimestampFormat { get; set; }

    /// <summary>
    /// Gets or sets indication whether or not UTC timezone should be used to for timestamps in logging messages. Defaults to <c>false</c>.
    /// </summary>
    public bool UseUtcTimestamp { get; set; }
}
