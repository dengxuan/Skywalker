// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;

namespace Skywalker.AspNetCore;

/// <summary>
/// The endpoint router
/// </summary>
public interface IEndpointRouter
{
    /// <summary>
    /// Finds a matching endpoint.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns></returns>
    IEndpointHandler? Find(HttpContext context);
}
