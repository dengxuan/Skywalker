using Skywalker.Ddd.Application.Dtos;

namespace Skywalker.Ddd.Application;

public interface ICommander
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask ExecuteAsync(object command, CancellationToken cancellationToken = default);
}
