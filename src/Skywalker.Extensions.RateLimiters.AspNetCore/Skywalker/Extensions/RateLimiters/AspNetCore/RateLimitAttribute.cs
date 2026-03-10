// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.RateLimiters.AspNetCore;

/// <summary>
/// Attribute to apply rate limiting to an endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class RateLimitAttribute : Attribute
{
    /// <summary>
    /// Gets the name of the rate limiting policy to apply.
    /// </summary>
    public string PolicyName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimitAttribute"/> class.
    /// </summary>
    /// <param name="policyName">The name of the policy to apply.</param>
    public RateLimitAttribute(string policyName)
    {
        PolicyName = policyName.NotNullOrEmpty(nameof(policyName));
    }
}

/// <summary>
/// Attribute to disable rate limiting for an endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class DisableRateLimitingAttribute : Attribute
{
}

