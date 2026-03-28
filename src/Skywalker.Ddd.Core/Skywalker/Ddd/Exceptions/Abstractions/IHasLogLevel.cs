using Microsoft.Extensions.Logging;

namespace Skywalker.Ddd.Exceptions;

/// <summary>
/// Interface for exceptions that have a specific log level.
/// </summary>
public interface IHasLogLevel
{
    /// <summary>
    /// Gets the log level for this exception.
    /// </summary>
    LogLevel LogLevel { get; }
}

