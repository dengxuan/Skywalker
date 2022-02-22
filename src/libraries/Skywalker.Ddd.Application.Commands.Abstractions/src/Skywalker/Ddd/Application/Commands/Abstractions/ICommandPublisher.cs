using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Commands.Abstractions;

public interface ICommandPublisher<TCommand> where TCommand : IRequestDto
{
    Task PublishAsync(TCommand command, CancellationToken cancellationToken);
}
