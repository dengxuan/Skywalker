// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Skywalker.AspNetCore;

/// <summary>
/// Endpoint handler
/// </summary>
public interface IEndpointHandler
{
    /// <summary>
    /// Processes the request.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns></returns>
    Task<IEndpointResult> ProcessAsync(HttpContext context);
}
