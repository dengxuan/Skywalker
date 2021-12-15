using Skywalker.Ddd.Application.Dtos;

namespace Skywalker.Ddd.Application;

public class MissingHandlerException : Exception
{
    public IDto? Dto { get; }

    public MissingHandlerException(IDto? dto)
        : base("No handler reqistered for dto type: " + dto?.GetType().FullName ?? "Unknown")
    {
        Dto = dto;
    }
}
