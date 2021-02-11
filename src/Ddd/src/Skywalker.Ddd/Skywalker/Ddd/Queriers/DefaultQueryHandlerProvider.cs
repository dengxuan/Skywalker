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
        private readonly IServiceProvider _serviceProvider;

        public DefaultQueryHandlerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<TOutput> HandleAsync(CancellationToken cancellationToken)
        {
            IQueryHandler<TOutput> queryHandler = _serviceProvider.GetRequiredService<IQueryHandler<TOutput>>();
            Task<TOutput> Handler() => queryHandler.HandleAsync(cancellationToken);

            return _serviceProvider.GetServices<ISearchingPipelineBehavior<TOutput>>()
                                   .Reverse()
                                   .Aggregate((QueryHandlerDelegate<TOutput>)Handler, (next, pipeline) => () => pipeline.HandleAsync(next, cancellationToken))();
        }
    }

    public class DefaultQueryHandlerProvider<TQuery, TOutput> : IQueryHandlerProvider<TQuery, TOutput> where TQuery : IQuery<TOutput>
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultQueryHandlerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<TOutput> HandleAsync(TQuery querier, CancellationToken cancellationToken)
        {
            IQueryHandler<TQuery, TOutput> queryHandler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TOutput>>();
            Task<TOutput> Handler() => queryHandler.HandleAsync(querier, cancellationToken);

            return _serviceProvider.GetServices<ISearchingPipelineBehavior<TQuery, TOutput>>()
                                   .Reverse()
                                   .Aggregate((QueryHandlerDelegate<TOutput>)Handler, (next, pipeline) => () => pipeline.HandleAsync(querier, next, cancellationToken))();
        }
    }
}
