using Microsoft.AspNetCore.Http;
#if NETCOREAPP3_1_OR_GREATER
using System.Text.Json;
#else
using Newtonsoft.Json;
#endif

namespace Skywalker.AspNetCore.Endpoints;

internal class PullPermissionsResult : IEndpointResult
{
    private readonly IReadOnlyList<Permission> _permissions;

    public PullPermissionsResult(IReadOnlyList<Permission> permissions) => _permissions = permissions;

    public async Task ExecuteAsync(HttpContext context)
    {
#if NETCOREAPP3_1_OR_GREATER
        var json = JsonSerializer.Serialize(_permissions);
#elif NETSTANDARD2_0_OR_GREATER
        var json = JsonConvert.SerializeObject(_permissions);
#endif

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
