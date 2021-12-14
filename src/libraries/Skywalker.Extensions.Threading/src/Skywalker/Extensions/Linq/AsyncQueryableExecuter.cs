using System.Linq.Expressions;

namespace Skywalker.Extensions.Linq;

public class AsyncQueryableExecuter : IAsyncQueryableExecuter
{
    protected IEnumerable<IAsyncQueryableProvider> Providers { get; }

    public AsyncQueryableExecuter(IEnumerable<IAsyncQueryableProvider> providers)
    {
        Providers = providers;
    }

    protected virtual IAsyncQueryableProvider? FindProvider<T>(IQueryable<T> queryable)
    {
        return Providers.FirstOrDefault(p => p.CanExecute(queryable));
    }

    public async Task<bool> ContainsAsync<T>(IQueryable<T> queryable, T item, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Contains(item) : await provider.ContainsAsync(queryable, item, cancellationToken);
    }

    public async Task<bool> AnyAsync<T>(IQueryable<T> queryable, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Any(predicate) : await provider.AnyAsync(queryable, predicate, cancellationToken);
    }

    public async Task<bool> AllAsync<T>(IQueryable<T> queryable, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.All(predicate) : await provider.AllAsync(queryable, predicate, cancellationToken);
    }

    public async Task<int> CountAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Count() : await provider.CountAsync(queryable, cancellationToken);
    }

    public async Task<int> CountAsync<T>(IQueryable<T> queryable, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Count(predicate) : await provider.CountAsync(queryable, predicate, cancellationToken);
    }

    public async Task<long> LongCountAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.LongCount() : await provider.LongCountAsync(queryable, cancellationToken);
    }

    public async Task<long> LongCountAsync<T>(IQueryable<T> queryable, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.LongCount(predicate) : await provider.LongCountAsync(queryable, predicate, cancellationToken);
    }

    public async Task<T> FirstAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.First() : await provider.FirstAsync(queryable, cancellationToken);
    }

    public async Task<T> FirstAsync<T>(IQueryable<T> queryable, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.First(predicate) : await provider.FirstAsync(queryable, predicate, cancellationToken);
    }

    public async Task<T?> FirstOrDefaultAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.FirstOrDefault() : await provider.FirstOrDefaultAsync(queryable, cancellationToken);
    }

    public async Task<T?> FirstOrDefaultAsync<T>(IQueryable<T> queryable, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.FirstOrDefault(predicate) : await provider.FirstOrDefaultAsync(queryable, predicate, cancellationToken);
    }

    public async Task<T> LastAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Last() : await provider.LastAsync(queryable, cancellationToken);
    }

    public async Task<T> LastAsync<T>(IQueryable<T> queryable, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Last(predicate) : await provider.LastAsync(queryable, predicate, cancellationToken);
    }

    public async Task<T?> LastOrDefaultAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.LastOrDefault() : await provider.LastOrDefaultAsync(queryable, cancellationToken);
    }

    public async Task<T?> LastOrDefaultAsync<T>(IQueryable<T> queryable, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.LastOrDefault(predicate) : await provider.LastOrDefaultAsync(queryable, predicate, cancellationToken);
    }

    public async Task<T> SingleAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Single() : await provider.SingleAsync(queryable, cancellationToken);
    }

    public async Task<T> SingleAsync<T>(IQueryable<T> queryable, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Single(predicate) : await provider.SingleAsync(queryable, predicate, cancellationToken);
    }

    public async Task<T?> SingleOrDefaultAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.SingleOrDefault() : await provider.SingleOrDefaultAsync(queryable, cancellationToken);
    }

    public async Task<T?> SingleOrDefaultAsync<T>(IQueryable<T> queryable, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.SingleOrDefault(predicate) : await provider.SingleOrDefaultAsync(queryable, predicate, cancellationToken);
    }

    public async Task<T?> MinAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Min() : await provider.MinAsync(queryable, cancellationToken);
    }

    public async Task<TResult?> MinAsync<T, TResult>(IQueryable<T> queryable, Expression<Func<T, TResult?>> selector, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Min(selector) : await provider.MinAsync(queryable, selector, cancellationToken);
    }

    public async Task<T?> MaxAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Max() : await provider.MaxAsync(queryable, cancellationToken);
    }

    public async Task<TResult?> MaxAsync<T, TResult>(IQueryable<T> queryable, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Max(selector) : await provider.MaxAsync(queryable, selector, cancellationToken);
    }

    public async Task<decimal> SumAsync(IQueryable<decimal> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Sum() : await provider.SumAsync(queryable, cancellationToken);
    }

    public async Task<decimal?> SumAsync(IQueryable<decimal?> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Sum() : await provider.SumAsync(queryable, cancellationToken);
    }

    public async Task<decimal> SumAsync<T>(IQueryable<T> queryable, Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Sum(selector) : await provider.SumAsync(queryable, selector, cancellationToken);
    }

    public async Task<decimal?> SumAsync<T>(IQueryable<T> queryable, Expression<Func<T, decimal?>> selector, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Sum(selector) : await provider.SumAsync(queryable, selector, cancellationToken);
    }

    public async Task<int> SumAsync(IQueryable<int> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Sum() : await provider.SumAsync(queryable, cancellationToken);
    }

    public async Task<int?> SumAsync(IQueryable<int?> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Sum() : await provider.SumAsync(queryable, cancellationToken);
    }

    public async Task<int> SumAsync<T>(IQueryable<T> queryable, Expression<Func<T, int>> selector, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Sum(selector) : await provider.SumAsync(queryable, selector, cancellationToken);
    }

    public async Task<int?> SumAsync<T>(IQueryable<T> queryable, Expression<Func<T, int?>> selector, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Sum(selector) : await provider.SumAsync(queryable, selector, cancellationToken);
    }

    public async Task<long> SumAsync(IQueryable<long> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Sum() : await provider.SumAsync(queryable, cancellationToken);
    }

    public async Task<long?> SumAsync(IQueryable<long?> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Sum() : await provider.SumAsync(queryable, cancellationToken);
    }

    public async Task<long> SumAsync<T>(IQueryable<T> queryable, Expression<Func<T, long>> selector, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Sum(selector) : await provider.SumAsync(queryable, selector, cancellationToken);
    }

    public async Task<long?> SumAsync<T>(IQueryable<T> queryable, Expression<Func<T, long?>> selector, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Sum(selector) : await provider.SumAsync(queryable, selector, cancellationToken);
    }

    public async Task<double> SumAsync(IQueryable<double> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Sum() : await provider.SumAsync(queryable, cancellationToken);
    }

    public async Task<double?> SumAsync(IQueryable<double?> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Sum() : await provider.SumAsync(queryable, cancellationToken);
    }

    public async Task<double> SumAsync<T>(IQueryable<T> queryable, Expression<Func<T, double>> selector, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Sum(selector) : await provider.SumAsync(queryable, selector, cancellationToken);
    }

    public async Task<double?> SumAsync<T>(IQueryable<T> queryable, Expression<Func<T, double?>> selector, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Sum(selector) : await provider.SumAsync(queryable, selector, cancellationToken);
    }

    public async Task<float> SumAsync(IQueryable<float> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Sum() : await provider.SumAsync(queryable, cancellationToken);
    }

    public async Task<float?> SumAsync(IQueryable<float?> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Sum() : await provider.SumAsync(queryable, cancellationToken);
    }

    public async Task<float> SumAsync<T>(IQueryable<T> queryable, Expression<Func<T, float>> selector, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Sum(selector) : await provider.SumAsync(queryable, selector, cancellationToken);
    }

    public async Task<float?> SumAsync<T>(IQueryable<T> queryable, Expression<Func<T, float?>> selector, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Sum(selector) : await provider.SumAsync(queryable, selector, cancellationToken);
    }

    public async Task<decimal> AverageAsync(IQueryable<decimal> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Average() : await provider.AverageAsync(queryable, cancellationToken);
    }

    public async Task<decimal?> AverageAsync(IQueryable<decimal?> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Average() : await provider.AverageAsync(queryable, cancellationToken);
    }

    public async Task<decimal> AverageAsync<T>(IQueryable<T> queryable, Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Average(selector) : await provider.AverageAsync(queryable, selector, cancellationToken);
    }

    public async Task<decimal?> AverageAsync<T>(IQueryable<T> queryable, Expression<Func<T, decimal?>> selector, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Average(selector) : await provider.AverageAsync(queryable, selector, cancellationToken);
    }

    public async Task<double> AverageAsync(IQueryable<int> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Average() : await provider.AverageAsync(queryable, cancellationToken);
    }

    public async Task<double?> AverageAsync(IQueryable<int?> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Average() : await provider.AverageAsync(queryable, cancellationToken);
    }

    public async Task<double> AverageAsync<T>(IQueryable<T> queryable, Expression<Func<T, int>> selector, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Average(selector) : await provider.AverageAsync(queryable, selector, cancellationToken);
    }

    public async Task<double?> AverageAsync<T>(IQueryable<T> queryable, Expression<Func<T, int?>> selector, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Average(selector) : await provider.AverageAsync(queryable, selector, cancellationToken);
    }

    public async Task<double> AverageAsync(IQueryable<long> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Average() : await provider.AverageAsync(queryable, cancellationToken);
    }

    public async Task<double?> AverageAsync(IQueryable<long?> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Average() : await provider.AverageAsync(queryable, cancellationToken);
    }

    public async Task<double> AverageAsync<T>(IQueryable<T> queryable, Expression<Func<T, long>> selector, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Average(selector) : await provider.AverageAsync(queryable, selector, cancellationToken);
    }

    public async Task<double?> AverageAsync<T>(IQueryable<T> queryable, Expression<Func<T, long?>> selector, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Average(selector) : await provider.AverageAsync(queryable, selector, cancellationToken);
    }

    public async Task<double> AverageAsync(IQueryable<double> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider != null ? await provider.AverageAsync(queryable, cancellationToken) : queryable.Average();
    }

    public async Task<double?> AverageAsync(IQueryable<double?> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Average() : await provider.AverageAsync(queryable, cancellationToken);
    }

    public async Task<double> AverageAsync<T>(IQueryable<T> queryable, Expression<Func<T, double>> selector, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Average(selector) : await provider.AverageAsync(queryable, selector, cancellationToken);
    }

    public async Task<double?> AverageAsync<T>(IQueryable<T> queryable, Expression<Func<T, double?>> selector, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Average(selector) : await provider.AverageAsync(queryable, selector, cancellationToken);
    }

    public async Task<float> AverageAsync(IQueryable<float> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Average() : await provider.AverageAsync(queryable, cancellationToken);
    }

    public async Task<float?> AverageAsync(IQueryable<float?> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Average() : await provider.AverageAsync(queryable, cancellationToken);
    }

    public async Task<float> AverageAsync<T>(IQueryable<T> queryable, Expression<Func<T, float>> selector, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Average(selector) : await provider.AverageAsync(queryable, selector, cancellationToken);
    }

    public async Task<float?> AverageAsync<T>(IQueryable<T> queryable, Expression<Func<T, float?>> selector, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.Average(selector) : await provider.AverageAsync(queryable, selector, cancellationToken);
    }

    public async Task<List<T>> ToListAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.ToList() : await provider.ToListAsync(queryable, cancellationToken);
    }

    public async Task<T[]> ToArrayAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
    {
        var provider = FindProvider(queryable);
        return provider == null ? queryable.ToArray() : await provider.ToArrayAsync(queryable, cancellationToken);
    }
}
