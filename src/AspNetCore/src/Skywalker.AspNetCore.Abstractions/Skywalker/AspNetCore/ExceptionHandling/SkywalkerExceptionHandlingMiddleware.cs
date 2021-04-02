using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Skywalker.AspNetCore.Mvc.Models;
using Skywalker.ExceptionHandling;
using Skywalker.Extensions.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Skywalker.AspNetCore.ExceptionHandling
{
    public class SkywalkerExceptionHandlingMiddleware : IMiddleware, ITransientDependency
    {
        private readonly ILogger<SkywalkerExceptionHandlingMiddleware> _logger;

        private readonly Func<object, Task> _clearCacheHeadersDelegate;

        public SkywalkerExceptionHandlingMiddleware(ILogger<SkywalkerExceptionHandlingMiddleware> logger)
        {
            _logger = logger;

            _clearCacheHeadersDelegate = ClearCacheHeaders;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                // We can't do anything if the response has already started, just abort.
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("An exception occurred, but response has already started!");
                    throw;
                }

                await HandleAndWrapException(context, ex);
            }
        }

        private async Task HandleAndWrapException(HttpContext httpContext, Exception exception)
        {
            _logger.LogError(exception, "Internal Server Error!");

            var jsonSerializer = httpContext.RequestServices.GetRequiredService<IJsonSerializer>();
            var errorBuilder = httpContext.RequestServices.GetRequiredService<IErrorBuilder>();

            httpContext.Response.Clear();
            httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            httpContext.Response.OnStarting(_clearCacheHeadersDelegate, httpContext.Response);

            await httpContext.Response.WriteAsync(jsonSerializer.Serialize(new AjaxResponse(errorBuilder.BuildForException(exception))));

            await httpContext
                .RequestServices
                .GetRequiredService<IExceptionNotifier>()
                .NotifyAsync(
                    new ExceptionNotificationContext(exception)
                );
        }

        private Task ClearCacheHeaders(object state)
        {
            var response = (HttpResponse)state;

            response.Headers[HeaderNames.CacheControl] = "no-cache";
            response.Headers[HeaderNames.Pragma] = "no-cache";
            response.Headers[HeaderNames.Expires] = "-1";
            response.Headers.Remove(HeaderNames.ETag);

            return Task.CompletedTask;
        }
    }
}
