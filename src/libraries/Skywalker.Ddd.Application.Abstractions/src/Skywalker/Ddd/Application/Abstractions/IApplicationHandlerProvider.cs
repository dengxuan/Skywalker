using Skywalker.Ddd.Application.Dtos.Abstractions;
using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Ddd.Application.Abstractions;

public interface IApplicationHandlerProvider : ISingletonDependency
{
    ValueTask HandleAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequestDto;

    ValueTask<TResponse?> HandleAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequestDto where TResponse : IResponseDto;
}
