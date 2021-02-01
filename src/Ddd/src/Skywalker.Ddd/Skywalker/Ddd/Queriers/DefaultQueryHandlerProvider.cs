using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Queries.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.Queries
{

    public class DefaultQueryHandlerProvider<TOutput> : IQueryHandlerProvider<TOutput>
    {
        private readonly IQueryHandler<TOutput> _queryHandler;
        private readonly IEnumerable<ISearchingPipelineBehavior<TOutput>> _searchingPipelineBehaviors;

        public DefaultQueryHandlerProvider(IQueryHandler<TOutput> queryHandler, IEnumerable<ISearchingPipelineBehavior<TOutput>> searchingPipelineBehaviors)
        {
            _queryHandler = queryHandler;
            _searchingPipelineBehaviors = searchingPipelineBehaviors;
        }

        public Task<TOutput> HandleAsync(CancellationToken cancellationToken)
        {
            Task<TOutput> Handler() => _queryHandler.HandleAsync(cancellationToken);

            return _searchingPipelineBehaviors.Reverse()
                                              .Aggregate((QueryHandlerDelegate<TOutput>)Handler, (next, pipeline) => () => pipeline.HandleAsync(next, cancellationToken))();
        }
    }

    public class DefaultQueryHandlerProvider<TQuery, TOutput> : IQueryHandlerProvider<TQuery, TOutput> where TQuery : IQuery<TOutput>
    {
        private readonly IQueryHandler<TQuery, TOutput> _queryHandler;
        private readonly IEnumerable<ISearchingPipelineBehavior<TQuery, TOutput>> _searchingPipelineBehaviors;

        public DefaultQueryHandlerProvider(IQueryHandler<TQuery, TOutput> queryHandler, IEnumerable<ISearchingPipelineBehavior<TQuery, TOutput>> searchingPipelineBehaviors)
        {
            _queryHandler = queryHandler;
            _searchingPipelineBehaviors = searchingPipelineBehaviors;
        }

        public Task<TOutput> HandleAsync(TQuery querier, CancellationToken cancellationToken)
        {
            Task<TOutput> Handler() => _queryHandler.HandleAsync(querier, cancellationToken);

            return _searchingPipelineBehaviors.Reverse()
                                              .Aggregate((QueryHandlerDelegate<TOutput>)Handler, (next, pipeline) => () => pipeline.HandleAsync(querier, next, cancellationToken))();
        }
    }
}
