using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Queries.Abstractions
{
    public interface IQueryHandler<TOutput>
    {
        Task<TOutput> HandleAsync(CancellationToken cancellationToken);
    }

    public interface IQueryHandler<in TQuery, TOutput> where TQuery : IQuery<TOutput>
    {
        Task<TOutput> HandleAsync(TQuery query, CancellationToken cancellationToken);
    }
}
