using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Queries.Abstractions;

public interface IQueryHandlerProvider<TRequest, TResponse> where TRequest : IRequestDto where TResponse : IResponseDto
{
    ValueTask<TResponse?> HandleAsync(TRequest query, CancellationToken cancellationToken = default);
}
