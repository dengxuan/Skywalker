// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Skywalker.Extensions.RateLimiters.AspNetCore;

/// <summary>
/// Middleware for rate limiting HTTP requests.
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RateLimitingOptions _options;
    private readonly ILogger<RateLimitingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimitingMiddleware"/> class.
    /// </summary>
    public RateLimitingMiddleware(
        RequestDelegate next,
        IOptions<RateLimitingOptions> options,
        ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var rateLimitAttribute = endpoint?.Metadata.GetMetadata<RateLimitAttribute>();
        var disableRateLimiting = endpoint?.Metadata.GetMetadata<DisableRateLimitingAttribute>();

        // Skip if rate limiting is disabled for this endpoint
        if (disableRateLimiting != null)
        {
            await _next(context);
            return;
        }

        // Get the policy
        var policyName = rateLimitAttribute?.PolicyName ?? _options.DefaultPolicyName;
        var policy = _options.GetPolicy(policyName);

        if (policy == null)
        {
            await _next(context);
            return;
        }

        // Get the key for rate limiting
        var key = GetKey(context, rateLimitAttribute);
        var rateLimiter = policy.GetRateLimiter(key);

        // Try to acquire a permit
        var result = await rateLimiter.TryAcquireAsync(cancellationToken: context.RequestAborted);

        if (result.IsAcquired)
        {
            await _next(context);
            return;
        }

        // Request was rate limited
        _logger.LogWarning("Request rate limited. Policy: {PolicyName}, Key: {Key}", policyName, key);

        await policy.OnRejectedAsync(key, result);

        if (_options.OnRejected != null)
        {
            await _options.OnRejected(context, result);
        }
        else
        {
            context.Response.StatusCode = _options.RejectionStatusCode;
            if (result.RetryAfter > TimeSpan.Zero)
            {
                context.Response.Headers.RetryAfter = ((int)result.RetryAfter.TotalSeconds).ToString();
            }
            await context.Response.WriteAsync(result.Reason ?? "Too many requests. Please try again later.");
        }
    }

    private string GetKey(HttpContext context, RateLimitAttribute? attribute)
    {
        if (_options.KeyResolver != null)
        {
            return _options.KeyResolver(context);
        }

        // Default: use IP address
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}

