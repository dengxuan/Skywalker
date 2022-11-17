using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.AspNetCore.Security.Abstractions;
using Skywalker.AspNetCore.Security.Streams;

namespace Skywalker.AspNetCore.Security;
#if NETSTANDARD
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
        if (!_provider.CheckRequestAcceptsEncryption(httpContext))
        {
            await _next(httpContext);
            return;
        }

        var bodyStream = httpContext.Response.Body;
        var originalBufferFeature = httpContext.Features.Get<IHttpBufferingFeature>();
        var originalSendFileFeature = httpContext.Features.Get<IHttpSendFileFeature>();

        using var bodyWrapperStream = new BodyWrapperStream(httpContext, bodyStream, _provider, originalBufferFeature, originalSendFileFeature);
        httpContext.Response.Body = bodyWrapperStream;
        httpContext.Features.Set<IHttpBufferingFeature>(bodyWrapperStream);
        if (originalSendFileFeature != null)
        {
            httpContext.Features.Set<IHttpSendFileFeature>(bodyWrapperStream);
        }

        try
        {
            await _next(httpContext);
            // This is not disposed via a using statement because we don't want to flush the compression buffer for unhandled exceptions,
            // that may cause secondary exceptions.
            //bodyWrapperStream.Dispose();
        }
        finally
        {
            httpContext.Response.Body = bodyStream;
            httpContext.Features.Set(originalBufferFeature);
            if (originalSendFileFeature != null)
            {
                httpContext.Features.Set(originalSendFileFeature);
            }
        }
    }
}
#endif
