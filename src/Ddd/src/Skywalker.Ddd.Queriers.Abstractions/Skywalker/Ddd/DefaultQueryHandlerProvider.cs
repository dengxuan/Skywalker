using Microsoft.Extensions.DependencyInjection;
using Skywalker.Queries.Abstractions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Queries
{

    public class DefaultQueryHandlerProvider<TOutput> : IQueryHandlerProvider<TOutput>
    {
        private readonly IServiceProvider _iocResolver;

        public DefaultQueryHandlerProvider(IServiceProvider iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public Task<TOutput> HandleAsync(CancellationToken cancellationToken)
        {
            IQueryHandler<TOutput> handler = _iocResolver.GetRequiredService<IQueryHandler<TOutput>>();

            Task<TOutput> Handler() => handler.HandleAsync(cancellationToken);

            return _iocResolver
                .GetServices<ISearchingPipelineBehavior<TOutput>>()
                .Reverse()
                .Aggregate((QueryHandlerDelegate<TOutput>)Handler, (next, pipeline) => () => pipeline.HandleAsync(cancellationToken, next))();
        }
    }

    public class DefaultQueryHandlerProvider<TQuery, TOutput> : IQueryHandlerProvider<TQuery, TOutput> where TQuery : IQuery<TOutput>
    {
        private readonly IServiceProvider _iocResolver;

        public DefaultQueryHandlerProvider(IServiceProvider iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public Task<TOutput> HandleAsync(TQuery querier, CancellationToken cancellationToken)
        {
                IQueryHandler<TQuery, TOutput> handler = _iocResolver.GetRequiredService<IQueryHandler<TQuery, TOutput>>();

                Task<TOutput> Handler() => handler.HandleAsync(querier, cancellationToken);

                return _iocResolver.GetServices<ISearchingPipelineBehavior<TQuery, TOutput>>()
                            .Reverse()
                            .Aggregate((QueryHandlerDelegate<TOutput>)Handler, (next, pipeline) => () => pipeline.HandleAsync(querier, cancellationToken, next))();
        }
    }
}
