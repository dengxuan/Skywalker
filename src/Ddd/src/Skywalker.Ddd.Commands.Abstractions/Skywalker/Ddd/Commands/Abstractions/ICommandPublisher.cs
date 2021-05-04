using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.Commands.Abstractions
{
    public interface ICommandPublisher<TCommand> where TCommand : ICommand
    {
        Task PublishAsync(TCommand command, CancellationToken cancellationToken);
    }
}
