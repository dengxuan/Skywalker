using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Application.Commands.Abstractions;
using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Commands;

public class DefaultCommandPublisher<TCommand> : ICommandPublisher<TCommand> where TCommand : IRequestDto
{
    private readonly IServiceProvider _iocResolver;

    public DefaultCommandPublisher(IServiceProvider iocResolver)
    {
        _iocResolver = iocResolver;
    }

    public Task PublishAsync(TCommand command, CancellationToken cancellationToken)
    {
        var handlers = _iocResolver
            .GetServices<ICommandHandler<TCommand>>()
            .Select(x => x.HandleAsync(command, cancellationToken));

        return Task.WhenAll(handlers);
    }
}
