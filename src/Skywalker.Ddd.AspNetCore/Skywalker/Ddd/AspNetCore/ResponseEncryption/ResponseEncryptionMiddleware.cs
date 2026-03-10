// Licensed to the Gordon

using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Skywalker.Ddd.AspNetCore.ResponseEncryption;

/// <summary>
/// Enable HTTP response encryption.
/// </summary>
public class ResponseEncryptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IResponseEncryptionProvider _provider;

    /// <summary>
    /// Initialize the Response encryption middleware.
    /// </summary>
    /// <param name="next">The delegate representing the remaining middleware in the request pipeline.</param>
    /// <param name="provider">The <see cref="IResponseEncryptionProvider"/>.</param>
    public ResponseEncryptionMiddleware(RequestDelegate next, IResponseEncryptionProvider provider)
    {
        ArgumentNullException.ThrowIfNull(next);
        ArgumentNullException.ThrowIfNull(provider);

        _next = next;
        _provider = provider;
    }

    /// <summary>
    /// Invoke the middleware.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/>.</param>
    /// <returns>A task that represents the execution of this middleware.</returns>
    public Task Invoke(HttpContext context)
    {
        return InvokeCore(context);
    }

    private async Task InvokeCore(HttpContext context)
    {
        var originalBodyFeature = context.Features.Get<IHttpResponseBodyFeature>();
        //var originalCompressionFeature = context.Features.Get<IHttpsCompressionFeature>();

        Debug.Assert(originalBodyFeature != null);

        var encryptionBody = new ResponseEncryptionBody(context, _provider, originalBodyFeature);
        context.Features.Set<IHttpResponseBodyFeature>(encryptionBody);
        //context.Features.Set<IHttpsCompressionFeature>(encryptionBody);
        try
        {
            await _next(context);
            await context.Response.CompleteAsync();
        }
        finally
        {
            context.Features.Set(originalBodyFeature);
            //context.Features.Set(originalCompressionFeature);
        }
    }
}
