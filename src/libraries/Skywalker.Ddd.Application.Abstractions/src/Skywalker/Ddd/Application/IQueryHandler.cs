using Skywalker.Ddd.Application.Dtos;

namespace Skywalker.Ddd.Application;

public interface IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    ValueTask<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken);
}
