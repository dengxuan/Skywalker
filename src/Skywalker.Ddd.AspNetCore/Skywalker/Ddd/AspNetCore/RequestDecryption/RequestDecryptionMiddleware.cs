// Licensed to the Gordon

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.Extensions.Logging;

namespace Skywalker.Ddd.AspNetCore.RequestDecryption;

/// <summary>
/// Enables HTTP request decryption.
/// </summary>
internal sealed partial class RequestDecryptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestDecryptionMiddleware> _logger;
    private readonly IRequestDecryptionProvider _provider;

    /// <summary>
    /// Initialize the request decryption middleware.
    /// </summary>
    /// <param name="next">The delegate representing the remaining middleware in the request pipeline.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="provider">The <see cref="IRequestDecryptionProvider"/>.</param>
    public RequestDecryptionMiddleware(
        RequestDelegate next,
        ILogger<RequestDecryptionMiddleware> logger,
        IRequestDecryptionProvider provider)
    {
        ArgumentNullException.ThrowIfNull(next);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(provider);

        _next = next;
        _logger = logger;
        _provider = provider;
    }

    /// <summary>
    /// Invoke the middleware.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/>.</param>
    /// <returns>A task that represents the execution of this middleware.</returns>
    public async Task Invoke(HttpContext context)
    {
        var decryptionStream = await _provider.GetDecryptionStream(context);
        if (decryptionStream is null)
        {
            await _next(context);
            return;
        }

        await InvokeCore(context, decryptionStream);
    }

    private async Task InvokeCore(HttpContext context, Stream decryptionStream)
    {
        var request = context.Request.Body;
        try
        {
            var sizeLimit = context.GetEndpoint()?.Metadata?.GetMetadata<IRequestSizeLimitMetadata>()?.MaxRequestBodySize
                ?? context.Features.Get<IHttpMaxRequestBodySizeFeature>()?.MaxRequestBodySize;

            context.Request.Body = new SizeLimitedStream(decryptionStream, sizeLimit);
            await _next(context);
        }
        finally
        {
            context.Request.Body = request;
            await decryptionStream.DisposeAsync();
        }
    }
}
