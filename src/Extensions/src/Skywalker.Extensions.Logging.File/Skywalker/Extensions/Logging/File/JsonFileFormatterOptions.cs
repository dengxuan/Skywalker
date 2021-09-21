// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;

namespace Skywalker.Extensions.Logging.File;

/// <summary>
/// Options for the built-in json console log formatter.
/// </summary>
public class JsonFileFormatterOptions : FileFormatterOptions
{
    public JsonFileFormatterOptions() { }

    /// <summary>
    /// Gets or sets JsonWriterOptions.
    /// </summary>
    public JsonWriterOptions JsonWriterOptions { get; set; }
}
