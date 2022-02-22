using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Pipeline;

public interface IPipelineBehavior<TMessage, TResponse> where TMessage : notnull, IRequestDto
{
    ValueTask<TResponse> HandleAsync(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken);
}
