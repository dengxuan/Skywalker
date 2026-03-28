using Microsoft.Extensions.Logging;

namespace Skywalker.Exceptions;

public interface IHasLogLevel
{
    LogLevel LogLevel { get; }
}
