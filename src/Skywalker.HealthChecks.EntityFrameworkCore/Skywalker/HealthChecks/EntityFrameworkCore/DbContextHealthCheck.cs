using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Skywalker.HealthChecks.EntityFrameworkCore;

/// <summary>
/// Health check for Entity Framework Core database connection.
/// </summary>
/// <typeparam name="TContext">The type of the DbContext.</typeparam>
public class DbContextHealthCheck<TContext> : IHealthCheck where TContext : DbContext
{
    private readonly TContext _dbContext;

    /// <summary>
    /// Initializes a new instance of <see cref="DbContextHealthCheck{TContext}"/>.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public DbContextHealthCheck(TContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);

            var data = new Dictionary<string, object>
            {
                { "database_provider", _dbContext.Database.ProviderName ?? "Unknown" },
                { "context_type", typeof(TContext).Name }
            };

            // Try to get connection string info (without sensitive data)
            var connectionString = _dbContext.Database.GetDbConnection().ConnectionString;
            if (!string.IsNullOrEmpty(connectionString))
            {
                data["has_connection_string"] = true;
            }

            if (!canConnect)
            {
                return HealthCheckResult.Unhealthy(
                    description: $"Cannot connect to database for {typeof(TContext).Name}",
                    data: data);
            }

            return HealthCheckResult.Healthy(
                description: $"Database connection for {typeof(TContext).Name} is healthy",
                data: data);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                description: $"Database connection for {typeof(TContext).Name} failed",
                exception: ex,
                data: new Dictionary<string, object>
                {
                    { "error", ex.Message },
                    { "context_type", typeof(TContext).Name }
                });
        }
    }
}

