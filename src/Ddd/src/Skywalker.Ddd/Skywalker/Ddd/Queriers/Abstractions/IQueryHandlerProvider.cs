using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.Queries.Abstractions
{
    public interface IQueryHandlerProvider<TOutput>
    {
        Task<TOutput> HandleAsync(CancellationToken cancellationToken = default);
    }

    public interface IQueryHandlerProvider<TQuery, TOutput> where TQuery : IQuery<TOutput>
    {
        Task<TOutput> HandleAsync(TQuery querier, CancellationToken cancellationToken = default);
    }
}
