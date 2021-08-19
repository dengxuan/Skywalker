using Skywalker.Ddd.Commands.Abstractions;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Skywalker.Ddd.Commands
{
    public class DefaultCommandHandlerProvider<TCommandHandler, TCommand> : ICommandHandlerProvider<TCommandHandler, TCommand> where TCommand : ICommand where TCommandHandler : ICommandHandler<TCommand>
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
}
