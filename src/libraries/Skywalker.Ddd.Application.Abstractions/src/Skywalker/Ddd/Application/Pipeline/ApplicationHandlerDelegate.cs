using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Pipeline;



public delegate ValueTask ApplicationHandlerDelegate<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : notnull, IRequestDto;


public delegate ValueTask<TResponse?> ApplicationHandlerDelegate<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken) where TRequest : notnull, IRequestDto where TResponse: notnull, IResponseDto;
