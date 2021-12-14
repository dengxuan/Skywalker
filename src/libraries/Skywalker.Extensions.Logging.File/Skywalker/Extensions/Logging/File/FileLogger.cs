// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Runtime.Versioning;

namespace Skywalker.Extensions.Logging.File;

#if NET5_0_OR_GREATER
[UnsupportedOSPlatform("browser")]
#endif
internal sealed class FileLogger : ILogger
{
    private readonly string _name;
    private readonly FileLoggerProcessor _queueProcessor;

    internal FileLogger(string name, FileLoggerProcessor loggerProcessor)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
        _queueProcessor = loggerProcessor;
    }

    internal FileFormatter? Formatter { get; set; }

    internal IExternalScopeProvider ScopeProvider { get; set; }

    internal FileLoggerOptions? Options { get; set; }

    [ThreadStatic]
    private static StringWriter? t_stringWriter;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }
        if (formatter == null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }
        t_stringWriter ??= new StringWriter();
        LogEntry<TState> logEntry = new(logLevel, _name, eventId, state, exception, formatter);
        Formatter?.Write(in logEntry, ScopeProvider, t_stringWriter);

        var sb = t_stringWriter.GetStringBuilder();
        if (sb.Length == 0)
        {
            return;
        }
        string computedAnsiString = sb.ToString();
        sb.Clear();
        if (sb.Capacity > 1024)
        {
            sb.Capacity = 1024;
        }
        _queueProcessor.EnqueueMessage(new LogMessageEntry(computedAnsiString, logAsError: logLevel >= Options?.LogToStandardErrorThreshold));
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? NullScope.Instance;
}
