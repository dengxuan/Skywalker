using Skywalker.Ddd.Application.Dtos;

namespace Skywalker.Ddd.Application.Pipeline;

public interface IPipelineBehavior<TMessage, TResponse> where TMessage : notnull, IDto
{
    ValueTask<TResponse> HandleAsync(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken);
}
