// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Skywalker.Extensions.Logging.File;

/// <summary>
/// A provider of <see cref="FileLogger"/> instances.
/// </summary>
#if NET5_0_OR_GREATER
[UnsupportedOSPlatform("browser")]
#endif
[ProviderAlias("Console")]
public class FileLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly IOptionsMonitor<FileLoggerOptions> _options;
    private readonly ConcurrentDictionary<string, FileLogger> _loggers;
    private readonly FileLoggerProcessor _messageQueue;
    private readonly IDisposable _optionsReloadToken;

    private ConcurrentDictionary<string, FileFormatter> _formatters;

    private IExternalScopeProvider _scopeProvider = NullExternalScopeProvider.Instance;
    private bool _disposedValue;

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
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public FileLoggerProvider(IOptionsMonitor<FileLoggerOptions> options, IEnumerable<FileFormatter> formatters)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        _options = options;
        _loggers = new ConcurrentDictionary<string, FileLogger>();
        SetFormatters(formatters);

        ReloadLoggerOptions(options.CurrentValue);
        _optionsReloadToken = _options.OnChange(ReloadLoggerOptions);

        _messageQueue = new FileLoggerProcessor(new AnsiLogFile(), new AnsiLogFile(stdErr: true));
    }

    private void SetFormatters(IEnumerable<FileFormatter>? formatters = null)
    {
        var cd = new ConcurrentDictionary<string, FileFormatter>(StringComparer.OrdinalIgnoreCase);

        var added = false;
        if (formatters != null)
        {
            foreach (var formatter in formatters)
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
        if (options.FormatterName == null || !_formatters.TryGetValue(options.FormatterName, out var fileFormatter))
        {
            fileFormatter = _formatters![FileFormatterNames.Simple];
        }

        foreach (var logger in _loggers)
        {
            logger.Value.Options = options;
            logger.Value.Formatter = fileFormatter;
        }
    }

    /// <inheritdoc />
    public ILogger CreateLogger(string name)
    {
        if (_options.CurrentValue.FormatterName == null || !_formatters.TryGetValue(_options.CurrentValue.FormatterName, out var fileFormatter))
        {
            fileFormatter = _formatters[FileFormatterNames.Simple];
        }

        return _loggers.TryGetValue(name, out var logger) ?
            logger :
            _loggers.GetOrAdd(name, new FileLogger(name, _messageQueue, _scopeProvider)
            {
                Options = _options.CurrentValue,
                Formatter = fileFormatter,
            });
    }

    /// <inheritdoc />
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider;

        foreach (var logger in _loggers)
        {
            logger.Value.ScopeProvider = _scopeProvider;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _optionsReloadToken?.Dispose();
                _messageQueue.Dispose();
            }
            _disposedValue = true;
        }
    }

    ~FileLoggerProvider()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
