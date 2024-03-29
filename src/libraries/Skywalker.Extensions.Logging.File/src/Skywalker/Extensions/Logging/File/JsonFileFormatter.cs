// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Skywalker.Extensions.Logging.File;

internal sealed class JsonFileFormatter : FileFormatter, IDisposable
{
    private readonly IDisposable _optionsReloadToken;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public JsonFileFormatter(IOptionsMonitor<JsonFileFormatterOptions> options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        : base(FileFormatterNames.Json)
    {
        ReloadLoggerOptions(options.CurrentValue);
        _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
    }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
    {
        var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);
        if (logEntry.Exception == null && message == null)
        {
            return;
        }
        var logLevel = logEntry.LogLevel;
        var category = logEntry.Category;
        var eventId = logEntry.EventId.Id;
        var exception = logEntry.Exception;
        const int DefaultBufferSize = 1024;
        using (var output = new PooledByteBufferWriter(DefaultBufferSize))
        {
            using (var writer = new Utf8JsonWriter(output, FormatterOptions.JsonWriterOptions))
            {
                writer.WriteStartObject();
                var timestampFormat = FormatterOptions.TimestampFormat;
                if (timestampFormat != null)
                {
                    var dateTimeOffset = FormatterOptions.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
                    writer.WriteString("Timestamp", dateTimeOffset.ToString(timestampFormat));
                }
                writer.WriteNumber(nameof(logEntry.EventId), eventId);
                writer.WriteString(nameof(logEntry.LogLevel), GetLogLevelString(logLevel));
                writer.WriteString(nameof(logEntry.Category), category);
                writer.WriteString("Message", message);

                if (exception != null)
                {
                    var exceptionMessage = exception.ToString();
                    if (!FormatterOptions.JsonWriterOptions.Indented)
                    {
                        exceptionMessage = exceptionMessage.Replace(Environment.NewLine, " ");
                    }
                    writer.WriteString(nameof(Exception), exceptionMessage);
                }

                if (logEntry.State != null)
                {
                    writer.WriteStartObject(nameof(logEntry.State));
                    writer.WriteString("Message", logEntry.State.ToString());
                    if (logEntry.State is IReadOnlyCollection<KeyValuePair<string, object>> stateProperties)
                    {
                        foreach (var item in stateProperties)
                        {
                            WriteItem(writer, item);
                        }
                    }
                    writer.WriteEndObject();
                }
                WriteScopeInformation(writer, scopeProvider);
                writer.WriteEndObject();
                writer.Flush();
            }
#if NETCOREAPP
            textWriter.Write(Encoding.UTF8.GetString(output.WrittenMemory.Span));
#else
            textWriter.Write(Encoding.UTF8.GetString(output.WrittenMemory.Span.ToArray()));
#endif
        }
        textWriter.Write(Environment.NewLine);
    }

    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "Trace",
            LogLevel.Debug => "Debug",
            LogLevel.Information => "Information",
            LogLevel.Warning => "Warning",
            LogLevel.Error => "Error",
            LogLevel.Critical => "Critical",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };
    }

    private void WriteScopeInformation(Utf8JsonWriter writer, IExternalScopeProvider scopeProvider)
    {
        if (FormatterOptions.IncludeScopes && scopeProvider != null)
        {
            writer.WriteStartArray("Scopes");
            scopeProvider.ForEachScope((scope, state) =>
            {
                if (scope is IEnumerable<KeyValuePair<string, object>> scopeItems)
                {
                    state.WriteStartObject();
                    state.WriteString("Message", scope.ToString());
                    foreach (var item in scopeItems)
                    {
                        WriteItem(state, item);
                    }
                    state.WriteEndObject();
                }
                else
                {
                    state.WriteStringValue(ToInvariantString(scope!));
                }
            }, writer);
            writer.WriteEndArray();
        }
    }

    private static void WriteItem(Utf8JsonWriter writer, KeyValuePair<string, object> item)
    {
        var key = item.Key;
        switch (item.Value)
        {
            case bool boolValue:
                writer.WriteBoolean(key, boolValue);
                break;
            case byte byteValue:
                writer.WriteNumber(key, byteValue);
                break;
            case sbyte sbyteValue:
                writer.WriteNumber(key, sbyteValue);
                break;
            case char charValue:
#if NETCOREAPP
                writer.WriteString(key, MemoryMarshal.CreateSpan(ref charValue, 1));
#else
                writer.WriteString(key, charValue.ToString());
#endif
                break;
            case decimal decimalValue:
                writer.WriteNumber(key, decimalValue);
                break;
            case double doubleValue:
                writer.WriteNumber(key, doubleValue);
                break;
            case float floatValue:
                writer.WriteNumber(key, floatValue);
                break;
            case int intValue:
                writer.WriteNumber(key, intValue);
                break;
            case uint uintValue:
                writer.WriteNumber(key, uintValue);
                break;
            case long longValue:
                writer.WriteNumber(key, longValue);
                break;
            case ulong ulongValue:
                writer.WriteNumber(key, ulongValue);
                break;
            case short shortValue:
                writer.WriteNumber(key, shortValue);
                break;
            case ushort ushortValue:
                writer.WriteNumber(key, ushortValue);
                break;
            case null:
                writer.WriteNull(key);
                break;
            default:
                writer.WriteString(key, ToInvariantString(item.Value));
                break;
        }
    }

    private static string? ToInvariantString(object obj) => Convert.ToString(obj, CultureInfo.InvariantCulture);

    internal JsonFileFormatterOptions FormatterOptions { get; set; }

    private void ReloadLoggerOptions(JsonFileFormatterOptions options)
    {
        FormatterOptions = options;
    }

    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
    }
}
