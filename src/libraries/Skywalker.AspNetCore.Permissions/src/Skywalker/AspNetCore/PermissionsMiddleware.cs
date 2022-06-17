// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Skywalker.AspNetCore.Endpoints;

namespace Skywalker.AspNetCore;

public class PermissionsMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<PermissionsMiddleware> _logger;

    public PermissionsMiddleware(RequestDelegate next, ILogger<PermissionsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context, IEndpointRouter router)
    {
        try
        {
            var endpoint = router.Find(context);
            if (endpoint == null)
            {
                await _next(context);
                return;
            }
            _logger.LogInformation("Invoking {endpoint} endpoint: {endpointType} for {url}", EndpointNames.CheckPermission, endpoint.GetType().FullName, context.Request.Path.ToString());
            
            var result = await endpoint.ProcessAsync(context);

            if (result == null)
            {
                return;
            }

            _logger.LogTrace("Invoking result: {type}", result.GetType().FullName);
            await result.ExecuteAsync(context);

        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unhandled exception: {exception}", ex.Message);
            throw;
        }
    }
}
