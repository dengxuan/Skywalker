using System.Collections.Generic;

namespace Skywalker.Ddd.Commands.Abstractions
{
    public interface ICommandHandlerProvider<TCommandHandler, out TCommand> where TCommand : ICommand where TCommandHandler : ICommandHandler<TCommand>
    {
        IEnumerable<TCommandHandler> CommandHandlers { get; }
    }
}
