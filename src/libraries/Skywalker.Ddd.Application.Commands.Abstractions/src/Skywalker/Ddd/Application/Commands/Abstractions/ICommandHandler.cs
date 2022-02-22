using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Commands.Abstractions;

public interface ICommandHandler<in TCommand> where TCommand : IRequestDto
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken);
}
