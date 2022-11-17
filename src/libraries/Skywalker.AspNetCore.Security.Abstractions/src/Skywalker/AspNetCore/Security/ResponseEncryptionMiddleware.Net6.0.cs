using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.AspNetCore.Security.Abstractions;

namespace Skywalker.AspNetCore.Security;
#if NET6_0
public class ResponseEncryptionMiddleware
{
    private readonly ILogger _logger;

    private readonly SafetyOptions _options;

    private readonly RequestDelegate _next;

    private readonly IResponseEncrpytionProvider _provider;

    public ResponseEncryptionMiddleware(RequestDelegate next, IOptions<SafetyOptions> options, IResponseEncrpytionProvider provider, ILoggerFactory loggerFactory)
    {
        _next = next;
        _options = options.Value;
        _provider = provider;
        _logger = loggerFactory.CreateLogger<ResponseEncryptionMiddleware>();
    }

    public async Task Invoke(HttpContext httpContext)
    {
        await _next(httpContext);
    }
}
#endif
