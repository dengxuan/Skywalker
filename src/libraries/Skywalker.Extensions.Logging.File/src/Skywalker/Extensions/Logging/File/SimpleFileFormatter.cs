// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace Skywalker.Extensions.Logging.File;

internal sealed class SimpleFileFormatter : FileFormatter, IDisposable
{
    private const string LoglevelPadding = ": ";
    private static readonly string s_messagePadding = new(' ', GetLogLevelString(LogLevel.Information).Length + LoglevelPadding.Length);
    private static readonly string s_newLineWithMessagePadding = Environment.NewLine + s_messagePadding;
    private readonly IDisposable _optionsReloadToken;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SimpleFileFormatter(IOptionsMonitor<SimpleFileFormatterOptions> options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        : base(FileFormatterNames.Simple)
    {
        ReloadLoggerOptions(options.CurrentValue);
        _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
    }

    private void ReloadLoggerOptions(SimpleFileFormatterOptions options)
    {
        FormatterOptions = options;
    }

    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
    }

    internal SimpleFileFormatterOptions FormatterOptions { get; set; }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
    {
        var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);
        if (logEntry.Exception == null && message == null)
        {
            return;
        }
        var logLevel = logEntry.LogLevel;
        var logLevelString = GetLogLevelString(logLevel);

        string? timestamp = null;
        var timestampFormat = FormatterOptions.TimestampFormat;
        if (timestampFormat != null)
        {
            var dateTimeOffset = GetCurrentDateTime();
            timestamp = dateTimeOffset.ToString(timestampFormat);
        }
        if (timestamp != null)
        {
            textWriter.Write(timestamp);
        }
        if (logLevelString != null)
        {
            textWriter.Write(message);
        }
        CreateDefaultLogMessage(textWriter, logEntry, message, scopeProvider);
    }

    private void CreateDefaultLogMessage<TState>(TextWriter textWriter, in LogEntry<TState> logEntry, string? message, IExternalScopeProvider scopeProvider)
    {
        var singleLine = FormatterOptions.SingleLine;
        var eventId = logEntry.EventId.Id;
        var exception = logEntry.Exception;

        // Example:
        // info: ConsoleApp.Program[10]
        //       Request received

        // category and event id
        textWriter.Write(LoglevelPadding);
        textWriter.Write(logEntry.Category);
        textWriter.Write('[');

#if NETCOREAPP
        Span<char> span = stackalloc char[10];
        if (eventId.TryFormat(span, out var charsWritten))
        {
            textWriter.Write(span.Slice(0, charsWritten));
        }
        else
        {
            textWriter.Write(eventId.ToString());
        }
#else

        textWriter.Write(eventId.ToString());
#endif

        textWriter.Write(']');
        if (!singleLine)
        {
            textWriter.Write(Environment.NewLine);
        }

        // scope information
        WriteScopeInformation(textWriter, scopeProvider, singleLine);
        WriteMessage(textWriter, message, singleLine);

        // Example:
        // System.InvalidOperationException
        //    at Namespace.Class.Function() in File:line X
        if (exception != null)
        {
            // exception message
            WriteMessage(textWriter, exception.ToString(), singleLine);
        }
        if (singleLine)
        {
            textWriter.Write(Environment.NewLine);
        }
    }

    private static void WriteMessage(TextWriter textWriter, string? message, bool singleLine)
    {
        if (!string.IsNullOrEmpty(message))
        {
            if (singleLine)
            {
                textWriter.Write(' ');
                WriteReplacing(textWriter, Environment.NewLine, " ", message);
            }
            else
            {
                textWriter.Write(s_messagePadding);
                WriteReplacing(textWriter, Environment.NewLine, s_newLineWithMessagePadding, message);
                textWriter.Write(Environment.NewLine);
            }
        }

        static void WriteReplacing(TextWriter writer, string oldValue, string newValue, string? message)
        {
            var newMessage = message?.Replace(oldValue, newValue);
            writer.Write(newMessage);
        }
    }

    private DateTimeOffset GetCurrentDateTime()
    {
        return FormatterOptions.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
    }

    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };
    }

    private void WriteScopeInformation(TextWriter textWriter, IExternalScopeProvider scopeProvider, bool singleLine)
    {
        if (FormatterOptions.IncludeScopes && scopeProvider != null)
        {
            var paddingNeeded = !singleLine;
            scopeProvider.ForEachScope((scope, state) =>
            {
                if (paddingNeeded)
                {
                    paddingNeeded = false;
                    state.Write(s_messagePadding);
                    state.Write("=> ");
                }
                else
                {
                    state.Write(" => ");
                }
                state.Write(scope);
            }, textWriter);

            if (!paddingNeeded && !singleLine)
            {
                textWriter.Write(Environment.NewLine);
            }
        }
    }

    private readonly struct ConsoleColors
    {
        public ConsoleColors(ConsoleColor? foreground, ConsoleColor? background)
        {
            Foreground = foreground;
            Background = background;
        }

        public ConsoleColor? Foreground { get; }

        public ConsoleColor? Background { get; }
    }
}
