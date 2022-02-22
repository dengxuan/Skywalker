using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Queries.Abstractions;

public interface IQueryHandler<in TRequest, TResponse> where TRequest : IRequestDto where TResponse : IResponseDto
{
    ValueTask<TResponse?> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}
