using Microsoft.Extensions.Logging;

namespace Skywalker.Logging
{
    public interface IExceptionWithSelfLogging
    {
        void Log(ILogger logger);
    }
}
