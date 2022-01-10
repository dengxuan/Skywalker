// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace Skywalker.Extensions.Logging.File;

/// <summary>
/// Options for a <see cref="FileLogger"/>.
/// </summary>
public class FileLoggerOptions
{
    /// <summary>
    /// Name of the log message formatter to use. Defaults to "simple" />.
    /// </summary>
    public string FormatterName { get; set; } = "simple";

    /// <summary>
    /// Gets or sets value indicating the minimum level of messages that would get written to <c>Console.Error</c>.
    /// </summary>
    public LogLevel LogToStandardErrorThreshold { get; set; } = LogLevel.Error;
}
