using Skywalker.Ddd.Application.Dtos;

namespace Skywalker.Ddd.Application.Pipeline;

public interface IPipelineBehavior<TMessage, TResponse> where TMessage : notnull, IDto
{
    ValueTask<TResponse> Handle(TMessage message, CancellationToken cancellationToken, MessageHandlerDelegate<TMessage, TResponse> next);
}
