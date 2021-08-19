using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.Commands.Abstractions
{

    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand command, CancellationToken cancellationToken);
    }
}
