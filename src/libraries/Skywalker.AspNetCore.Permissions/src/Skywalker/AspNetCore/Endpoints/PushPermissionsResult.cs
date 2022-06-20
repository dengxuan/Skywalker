using Microsoft.AspNetCore.Http;

namespace Skywalker.AspNetCore.Endpoints;

internal class PushPermissionsResult : IEndpointResult
{

    public Task ExecuteAsync(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status204NoContent;
        return Task.CompletedTask;
    }
}
