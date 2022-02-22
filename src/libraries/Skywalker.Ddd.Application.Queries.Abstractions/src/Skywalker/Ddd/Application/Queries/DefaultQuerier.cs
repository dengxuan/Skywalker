using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Application.Dtos.Abstractions;
using Skywalker.Ddd.Application.Queries.Abstractions;

namespace Skywalker.Ddd.Application.Queries;

public class DefaultQuerier : IQuerier
{
    private readonly IServiceProvider _iocResolver;

    public DefaultQuerier(IServiceProvider iocResolver)
    {
        _iocResolver = iocResolver;
    }

    public ValueTask<TResponse?> QueryAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequestDto where TResponse : IResponseDto
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var handler = _iocResolver.GetRequiredService<IQueryHandlerProvider<TRequest, TResponse>>();
        return handler.HandleAsync(request, cancellationToken);
    }
}
