// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;

namespace Skywalker.AspNetCore.PermissionsEvaluator.Endpoints;

internal class ChackPermissionResult : IEndpointResult
{
    private readonly string _name;

    private readonly bool _isGranted;

    public ChackPermissionResult(string name, bool isGranted)
    {
        _name = name;
        _isGranted = isGranted;
    }

    public async Task ExecuteAsync(HttpContext context)
    {
        var json = @$"{{ ""name"": {_name}, ""isGranted"": {_isGranted} }}";
        
        var response = context.Response;
        if (!response.Headers.ContainsKey("Cache-Control"))
        {
            response.Headers.Add("Cache-Control", "no-store, no-cache, max-age=0");
        }
        else
        {
            response.Headers["Cache-Control"] = "no-store, no-cache, max-age=0";
        }

        if (!response.Headers.ContainsKey("Pragma"))
        {
            response.Headers.Add("Pragma", "no-cache");
        }
        await context.Response.WriteAsync(json);
    }
}
