using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Commands.Abstractions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.Commands
{
    public class DefaultCommandPublisher<TCommand> : ICommandPublisher<TCommand> where TCommand : ICommand
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
}
