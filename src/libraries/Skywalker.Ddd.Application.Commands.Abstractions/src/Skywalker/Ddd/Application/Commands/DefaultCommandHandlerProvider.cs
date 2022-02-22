using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Application.Commands.Abstractions;
using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Commands;

public class DefaultCommandHandlerProvider<TCommandHandler, TCommand> : ICommandHandlerProvider<TCommandHandler, TCommand> where TCommandHandler : ICommandHandler<TCommand> where TCommand : IRequestDto
{
    private readonly IServiceProvider _iocResolver;

    public DefaultCommandHandlerProvider(IServiceProvider iocResolver)
    {
        _iocResolver = iocResolver;
    }

    public IEnumerable<TCommandHandler> CommandHandlers
    {
        get
        {
            return (IEnumerable<TCommandHandler>)_iocResolver.GetServices<ICommandHandler<TCommand>>();
        }
    }
}
