// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.RateLimiters;

/// <summary>
/// Represents a rate limiter policy.
/// </summary>
public interface IRateLimiterPolicy
{
    /// <summary>
    /// Gets the policy name.
    /// </summary>
    string PolicyName { get; }

    /// <summary>
    /// Gets a rate limiter for the specified key.
    /// </summary>
    /// <param name="key">The key to identify the rate limiter.</param>
    /// <returns>The rate limiter for the key.</returns>
    IRateLimiter GetRateLimiter(string key);

    /// <summary>
    /// Called when a request is rejected.
    /// </summary>
    /// <param name="key">The key that was rejected.</param>
    /// <param name="result">The rate limit result.</param>
    /// <returns>A task representing the operation.</returns>
    ValueTask OnRejectedAsync(string key, RateLimitResult result);
}

/// <summary>
/// Base class for rate limiter policies.
/// </summary>
/// <typeparam name="TOptions">The options type.</typeparam>
public abstract class RateLimiterPolicy<TOptions> : IRateLimiterPolicy where TOptions : RateLimiterOptions
{
    private readonly Dictionary<string, IRateLimiter> _limiters = new();
    private readonly object _lock = new();

    /// <summary>
    /// Gets the options.
    /// </summary>
    protected TOptions Options { get; }

    /// <summary>
    /// Initializes a new instance of the policy.
    /// </summary>
    /// <param name="policyName">The policy name.</param>
    /// <param name="options">The options.</param>
    protected RateLimiterPolicy(string policyName, TOptions options)
    {
        PolicyName = policyName.NotNull(nameof(policyName));
        Options = options.NotNull(nameof(options));
    }

    /// <inheritdoc />
    public string PolicyName { get; }

    /// <inheritdoc />
    public IRateLimiter GetRateLimiter(string key)
    {
        key.NotNull(nameof(key));

        lock (_lock)
        {
            if (!_limiters.TryGetValue(key, out var limiter))
            {
                limiter = CreateRateLimiter(key);
                _limiters[key] = limiter;
            }
            return limiter;
        }
    }

    /// <summary>
    /// Creates a rate limiter for the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The rate limiter.</returns>
    protected abstract IRateLimiter CreateRateLimiter(string key);

    /// <inheritdoc />
    public virtual ValueTask OnRejectedAsync(string key, RateLimitResult result)
    {
        return ValueTask.CompletedTask;
    }
}

/// <summary>
/// Fixed window rate limiter policy.
/// </summary>
public class FixedWindowRateLimiterPolicy : RateLimiterPolicy<FixedWindowRateLimiterOptions>
{
    /// <summary>
    /// Initializes a new instance of the policy.
    /// </summary>
    public FixedWindowRateLimiterPolicy(string policyName, FixedWindowRateLimiterOptions options)
        : base(policyName, options) { }

    /// <inheritdoc />
    protected override IRateLimiter CreateRateLimiter(string key)
    {
        return new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
        {
            Name = $"{PolicyName}:{key}",
            PermitLimit = Options.PermitLimit,
            Window = Options.Window,
            AutoReplenishment = Options.AutoReplenishment
        });
    }
}

/// <summary>
/// Sliding window rate limiter policy.
/// </summary>
public class SlidingWindowRateLimiterPolicy : RateLimiterPolicy<SlidingWindowRateLimiterOptions>
{
    /// <summary>
    /// Initializes a new instance of the policy.
    /// </summary>
    public SlidingWindowRateLimiterPolicy(string policyName, SlidingWindowRateLimiterOptions options)
        : base(policyName, options) { }

    /// <inheritdoc />
    protected override IRateLimiter CreateRateLimiter(string key)
    {
        return new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
        {
            Name = $"{PolicyName}:{key}",
            PermitLimit = Options.PermitLimit,
            Window = Options.Window,
            SegmentsPerWindow = Options.SegmentsPerWindow,
            AutoReplenishment = Options.AutoReplenishment
        });
    }
}

/// <summary>
/// Token bucket rate limiter policy.
/// </summary>
public class TokenBucketRateLimiterPolicy : RateLimiterPolicy<TokenBucketRateLimiterOptions>
{
    /// <summary>
    /// Initializes a new instance of the policy.
    /// </summary>
    public TokenBucketRateLimiterPolicy(string policyName, TokenBucketRateLimiterOptions options)
        : base(policyName, options) { }

    /// <inheritdoc />
    protected override IRateLimiter CreateRateLimiter(string key)
    {
        return new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
        {
            Name = $"{PolicyName}:{key}",
            PermitLimit = Options.PermitLimit,
            ReplenishmentPeriod = Options.ReplenishmentPeriod,
            TokensPerPeriod = Options.TokensPerPeriod,
            AutoReplenishment = Options.AutoReplenishment
        });
    }
}

