using Skywalker.Ddd.Application.Dtos;

namespace Skywalker.Ddd.Application.Pipeline;

public delegate ValueTask<TResponse> MessageHandlerDelegate<TMessage, TResponse>(TMessage message, CancellationToken cancellationToken) where TMessage : notnull, IDto;
