// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Skywalker.Extensions.Logging.File;

/// <summary>
/// A provider of <see cref="FileLogger"/> instances.
/// </summary>
[UnsupportedOSPlatform("browser")]
[ProviderAlias("Console")]
public class FileLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly IOptionsMonitor<FileLoggerOptions> _options;
    private readonly ConcurrentDictionary<string, FileLogger> _loggers;
    private ConcurrentDictionary<string, FileFormatter> _formatters;
    private readonly FileLoggerProcessor _messageQueue;

    private IDisposable _optionsReloadToken;
    private IExternalScopeProvider _scopeProvider = NullExternalScopeProvider.Instance;

    /// <summary>
    /// Creates an instance of <see cref="FileLoggerProvider"/>.
    /// </summary>
    /// <param name="options">The options to create <see cref="FileLogger"/> instances with.</param>
    public FileLoggerProvider(IOptionsMonitor<FileLoggerOptions> options)
        : this(options, Array.Empty<FileFormatter>()) { }

    /// <summary>
    /// Creates an instance of <see cref="FileLoggerProvider"/>.
    /// </summary>
    /// <param name="options">The options to create <see cref="FileLogger"/> instances with.</param>
    /// <param name="formatters">Log formatters added for <see cref="FileLogger"/> insteaces.</param>
    public FileLoggerProvider(IOptionsMonitor<FileLoggerOptions> options, IEnumerable<FileFormatter> formatters)
    {
        _options = options;
        _loggers = new ConcurrentDictionary<string, FileLogger>();
        SetFormatters(formatters);

        ReloadLoggerOptions(options.CurrentValue);
        _optionsReloadToken = _options.OnChange(ReloadLoggerOptions);

        _messageQueue = new FileLoggerProcessor
        {
            File = new AnsiLogFile(),
            ErrorFile = new AnsiLogFile(stdErr: true)
        };
    }

    private void SetFormatters(IEnumerable<FileFormatter> formatters = null)
    {
        var cd = new ConcurrentDictionary<string, FileFormatter>(StringComparer.OrdinalIgnoreCase);

        bool added = false;
        if (formatters != null)
        {
            foreach (FileFormatter formatter in formatters)
            {
                cd.TryAdd(formatter.Name, formatter);
                added = true;
            }
        }

        if (!added)
        {
            cd.TryAdd(FileFormatterNames.Simple, new SimpleFileFormatter(new FormatterOptionsMonitor<SimpleFileFormatterOptions>(new SimpleFileFormatterOptions())));
            cd.TryAdd(FileFormatterNames.Systemd, new SystemdFileFormatter(new FormatterOptionsMonitor<FileFormatterOptions>(new FileFormatterOptions())));
            cd.TryAdd(FileFormatterNames.Json, new JsonFileFormatter(new FormatterOptionsMonitor<JsonFileFormatterOptions>(new JsonFileFormatterOptions())));
        }

        _formatters = cd;
    }

    // warning:  ReloadLoggerOptions can be called before the ctor completed,... before registering all of the state used in this method need to be initialized
    private void ReloadLoggerOptions(FileLoggerOptions options)
    {
        if (options.FormatterName == null || !_formatters.TryGetValue(options.FormatterName, out FileFormatter? logFormatter))
        {
            logFormatter = _formatters[FileFormatterNames.Simple];
        }

        foreach (KeyValuePair<string, FileLogger> logger in _loggers)
        {
            logger.Value.Options = options;
            logger.Value.Formatter = logFormatter;
        }
    }

    /// <inheritdoc />
    public ILogger CreateLogger(string name)
    {
        if (_options.CurrentValue.FormatterName == null || !_formatters.TryGetValue(_options.CurrentValue.FormatterName, out FileFormatter? logFormatter))
        {
            logFormatter = _formatters[FileFormatterNames.Simple];
        }

        return _loggers.TryGetValue(name, out FileLogger? logger) ?
            logger :
            _loggers.GetOrAdd(name, new FileLogger(name, _messageQueue)
            {
                Options = _options.CurrentValue,
                ScopeProvider = _scopeProvider,
                Formatter = logFormatter,
            });
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
        _messageQueue.Dispose();
    }

    /// <inheritdoc />
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider;

        foreach (KeyValuePair<string, FileLogger> logger in _loggers)
        {
            logger.Value.ScopeProvider = _scopeProvider;
        }
    }
}
