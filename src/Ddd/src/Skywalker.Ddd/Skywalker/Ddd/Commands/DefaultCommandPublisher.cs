using Skywalker.Ddd.Commands.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.Commands
{
    public class DefaultCommandPublisher<TCommand> : ICommandPublisher<TCommand> where TCommand : ICommand
    {
        private readonly IEnumerable<ICommandHandler<TCommand>> _commandHandlers;

        public DefaultCommandPublisher(IEnumerable<ICommandHandler<TCommand>> commandHandlers)
        {
            _commandHandlers = commandHandlers;
        }

        public Task PublishAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            var handlers = _commandHandlers.Select(x => x.HandleAsync(command, cancellationToken));

            return Task.WhenAll(handlers);
        }
    }
}
