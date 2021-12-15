using Skywalker.Ddd.Application.Dtos;

namespace Skywalker.Ddd.Application;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    ValueTask Handle(TCommand command, CancellationToken cancellationToken);
}