using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.Queries.Abstractions
{
    /// <summary>
    /// Represents an async continuation for the next task to execute in the pipeline
    /// </summary>
    /// <typeparam name="TOutput">Response type</typeparam>
    /// <returns>Awaitable task returning a <typeparamref name="TOutput"/></returns>
    public delegate Task<TOutput> QueryHandlerDelegate<TOutput>();

    /// <summary>
    /// Pipeline behavior to surround the inner handler.
    /// Implementations add additional behavior and await the next delegate.
    /// </summary>
    /// <typeparam name="TQuery">Request type</typeparam>
    /// <typeparam name="TOutput">Response type</typeparam>
    public interface ISearchingPipelineBehavior<in TQuery, TOutput> where TQuery : IQuery<TOutput>
    {
        /// <summary>
        /// Pipeline handler. Perform any additional behavior and await the <paramref name="next"/> delegate as necessary
        /// </summary>
        /// <param name="querier">Incoming request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
        /// <returns>Awaitable task returning the <typeparamref name="TOutput"/></returns>
        Task<TOutput> HandleAsync(TQuery querier, QueryHandlerDelegate<TOutput> next, CancellationToken cancellationToken = default);
    }


    /// <summary>
    /// Pipeline behavior to surround the inner handler.
    /// Implementations add additional behavior and await the next delegate.
    /// </summary>
    /// <typeparam name="TQuery">Request type</typeparam>
    /// <typeparam name="TOutput">Response type</typeparam>
    public interface ISearchingPipelineBehavior<TOutput>
    {
        /// <summary>
        /// Pipeline handler. Perform any additional behavior and await the <paramref name="next"/> delegate as necessary
        /// </summary>
        /// <param name="querier">Incoming request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
        /// <returns>Awaitable task returning the <typeparamref name="TOutput"/></returns>
        Task<TOutput> HandleAsync(QueryHandlerDelegate<TOutput> next, CancellationToken cancellationToken = default);
    }
}
