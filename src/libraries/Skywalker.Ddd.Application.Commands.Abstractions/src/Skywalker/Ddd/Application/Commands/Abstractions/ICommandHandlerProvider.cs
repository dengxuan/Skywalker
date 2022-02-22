using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Commands.Abstractions;

public interface ICommandHandlerProvider<TCommandHandler, out TCommand> where TCommand : IRequestDto where TCommandHandler : ICommandHandler<TCommand>
{
    IEnumerable<TCommandHandler> CommandHandlers { get; }
}
