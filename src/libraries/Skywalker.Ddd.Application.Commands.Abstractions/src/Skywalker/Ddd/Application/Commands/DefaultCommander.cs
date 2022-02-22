using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skywalker.Ddd.Application.Commands.Abstractions;
using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Commands;

public class DefaultCommander : ICommander
{
    private readonly IServiceProvider _iocResolver;

    private readonly ILogger<DefaultCommander> _logger;

    public DefaultCommander(IServiceProvider iocResolver, ILogger<DefaultCommander> logger)
    {
        _logger = logger;
        _iocResolver = iocResolver;
    }

    public async Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : IRequestDto
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }
        try
        {
            using var scope = _iocResolver.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<ICommandPublisher<TCommand>>();
            await handler.PublishAsync(command, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
}
