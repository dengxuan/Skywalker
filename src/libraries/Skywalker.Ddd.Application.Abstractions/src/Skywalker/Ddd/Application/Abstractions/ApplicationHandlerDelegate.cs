using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Pipeline;



public delegate Task ApplicationHandlerDelegate<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : notnull, IRequestDto;


public delegate Task<TResponse?> ApplicationHandlerDelegate<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken) where TRequest : notnull, IRequestDto where TResponse: notnull, IResponseDto;
