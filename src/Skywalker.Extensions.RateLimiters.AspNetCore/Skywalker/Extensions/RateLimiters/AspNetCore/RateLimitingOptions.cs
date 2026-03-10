// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;

namespace Skywalker.Extensions.RateLimiters.AspNetCore;

/// <summary>
/// Options for rate limiting middleware.
/// </summary>
public class RateLimitingOptions
{
    private readonly Dictionary<string, IRateLimiterPolicy> _policies = new();

    /// <summary>
    /// Gets the default policy name.
    /// </summary>
    public string? DefaultPolicyName { get; set; }

    /// <summary>
    /// Gets or sets the rejection status code. Default is 429 (Too Many Requests).
    /// </summary>
    public int RejectionStatusCode { get; set; } = 429;

    /// <summary>
    /// Gets or sets the key resolver. Default is IP address.
    /// </summary>
    public Func<HttpContext, string>? KeyResolver { get; set; }

    /// <summary>
    /// Gets or sets the callback when a request is rejected.
    /// </summary>
    public Func<HttpContext, RateLimitResult, Task>? OnRejected { get; set; }

    /// <summary>
    /// Adds a fixed window rate limiter policy.
    /// </summary>
    public RateLimitingOptions AddFixedWindowLimiter(string policyName, Action<FixedWindowRateLimiterOptions> configure)
    {
        var options = new FixedWindowRateLimiterOptions { Name = policyName };
        configure(options);
        _policies[policyName] = new FixedWindowRateLimiterPolicy(policyName, options);
        return this;
    }

    /// <summary>
    /// Adds a sliding window rate limiter policy.
    /// </summary>
    public RateLimitingOptions AddSlidingWindowLimiter(string policyName, Action<SlidingWindowRateLimiterOptions> configure)
    {
        var options = new SlidingWindowRateLimiterOptions { Name = policyName };
        configure(options);
        _policies[policyName] = new SlidingWindowRateLimiterPolicy(policyName, options);
        return this;
    }

    /// <summary>
    /// Adds a token bucket rate limiter policy.
    /// </summary>
    public RateLimitingOptions AddTokenBucketLimiter(string policyName, Action<TokenBucketRateLimiterOptions> configure)
    {
        var options = new TokenBucketRateLimiterOptions { Name = policyName };
        configure(options);
        _policies[policyName] = new TokenBucketRateLimiterPolicy(policyName, options);
        return this;
    }

    /// <summary>
    /// Adds a custom policy.
    /// </summary>
    public RateLimitingOptions AddPolicy(string policyName, IRateLimiterPolicy policy)
    {
        _policies[policyName] = policy;
        return this;
    }

    /// <summary>
    /// Gets a policy by name.
    /// </summary>
    internal IRateLimiterPolicy? GetPolicy(string? policyName)
    {
        if (string.IsNullOrEmpty(policyName))
        {
            return null;
        }
        return _policies.TryGetValue(policyName, out var policy) ? policy : null;
    }

    /// <summary>
    /// Gets the default policy.
    /// </summary>
    internal IRateLimiterPolicy? GetDefaultPolicy()
    {
        return GetPolicy(DefaultPolicyName);
    }
}

