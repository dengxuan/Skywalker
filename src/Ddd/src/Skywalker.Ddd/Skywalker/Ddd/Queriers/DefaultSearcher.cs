using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Queries.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.Queries
{
    public class DefaultSearcher : ISearcher
    {
        private readonly IServiceProvider _iocResolver;

        public DefaultSearcher(IServiceProvider iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public Task<TOutput> SearchAsync<TOutput>(CancellationToken cancellationToken = default)
        {
            var handler = _iocResolver.GetRequiredService<IQueryHandlerProvider<TOutput>>();
            return handler.HandleAsync(cancellationToken);
        }

        public Task<TOutput> SearchAsync<TQuery, TOutput>(TQuery querier, CancellationToken cancellationToken = default) where TQuery : IQuery<TOutput>
        {
            if (querier == null)
            {
                throw new ArgumentNullException(nameof(querier));
            }

            var handler = _iocResolver.GetRequiredService<IQueryHandlerProvider<TQuery, TOutput>>();
            return handler.HandleAsync(querier, cancellationToken);
        }
    }
}
