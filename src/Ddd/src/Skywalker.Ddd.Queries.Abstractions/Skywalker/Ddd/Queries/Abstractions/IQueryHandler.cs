using Skywalker.Application.Dtos.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.Queries.Abstractions
{
    public interface IQueryHandler<TOutput>
    {
        Task<TOutput?> HandleAsync(CancellationToken cancellationToken = default);
    }

    public interface IQueryHandler<in TQuery, TOutput> where TQuery : IEntityDto
    {
        Task<TOutput?> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
    }
}
