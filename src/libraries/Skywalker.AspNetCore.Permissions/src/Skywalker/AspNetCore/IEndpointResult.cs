// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;

namespace Skywalker.AspNetCore;

/// <summary>
/// Endpoint result
/// </summary>
public interface IEndpointResult
{
    /// <summary>
    /// Executes the result.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns></returns>
    Task ExecuteAsync(HttpContext context);
}
