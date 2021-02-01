using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.Commands.Abstractions
{
    public interface ICommander
    {
        /// <summary>
        /// Asynchronously send a command to a single handler
        /// </summary>
        /// <typeparam name="TCommand">Command type</typeparam>
        /// <param name="command">Command object</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>A task that represents the send operation. The task result contains the handler response</returns>
        Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand;
    }
}
