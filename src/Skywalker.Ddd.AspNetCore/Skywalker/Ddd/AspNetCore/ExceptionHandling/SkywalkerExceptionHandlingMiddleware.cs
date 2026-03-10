using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skywalker.Ddd.AspNetCore.Mvc.Models.Abstractions;
using Skywalker.Ddd.Exceptions;
using Skywalker.Ddd.Exceptions.Abstractions;

namespace Skywalker.Ddd.AspNetCore.ExceptionHandling;

/// <summary>
/// Middleware for handling exceptions and HTTP status codes in the ASP.NET Core pipeline.
/// </summary>
public class SkywalkerExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SkywalkerExceptionHandlingMiddleware> _logger;
    private readonly Func<object, Task> _clearCacheHeadersDelegate;

    /// <summary>
    /// Creates a new instance of <see cref="SkywalkerExceptionHandlingMiddleware"/>.
    /// </summary>
    public SkywalkerExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<SkywalkerExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _clearCacheHeadersDelegate = ClearCacheHeaders;
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            // Handle non-exception error status codes (401, 403, etc.)
            await HandleStatusCodeAsync(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Response has already started, cannot handle exception.");
                throw;
            }

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleStatusCodeAsync(HttpContext context)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        // Check if wrapping is disabled via header
        if (ShouldSkipWrapping(context))
        {
            return;
        }

        var statusCode = context.Response.StatusCode;

        var error = statusCode switch
        {
            StatusCodes.Status401Unauthorized => new Error($"{statusCode}", "未授权"),
            StatusCodes.Status403Forbidden => new Error($"{statusCode}", "拒绝访问"),
            StatusCodes.Status404NotFound => new Error($"{statusCode}", "资源不存在"),
            _ => null
        };

        if (error != null)
        {
            context.Response.ContentType = "application/json";
            context.Response.Headers["x-wrap-result"] = bool.TrueString;
            var result = new AjaxResponse(error, statusCode is StatusCodes.Status401Unauthorized or StatusCodes.Status403Forbidden);
            await context.Response.WriteAsJsonAsync(result);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        LogException(exception);

        await NotifyExceptionAsync(context, exception);

        var statusCode = GetStatusCode(exception);

        // Check if wrapping is disabled via header
        if (ShouldSkipWrapping(context))
        {
            // Return raw HTTP status code without wrapping
            context.Response.Clear();
            context.Response.OnStarting(_clearCacheHeadersDelegate, context.Response);
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var errorBuilder = context.RequestServices.GetService<IErrorBuilder>();
            var error = errorBuilder?.BuildForException(exception) ?? new Error($"{(int)statusCode}", exception.Message);
            await context.Response.WriteAsJsonAsync(error);
            return;
        }

        // Wrap response with AjaxResponse
        var wrappedErrorBuilder = context.RequestServices.GetService<IErrorBuilder>();
        var wrappedError = wrappedErrorBuilder?.BuildForException(exception) ?? new Error($"{(int)statusCode}", exception.Message);

        context.Response.Clear();
        context.Response.OnStarting(_clearCacheHeadersDelegate, context.Response);
        context.Response.StatusCode = StatusCodes.Status200OK; // Always return 200 with AjaxResponse
        context.Response.ContentType = "application/json";
        context.Response.Headers["x-wrap-result"] = bool.TrueString;

        var isUnauthorized = statusCode is System.Net.HttpStatusCode.Unauthorized or System.Net.HttpStatusCode.Forbidden;
        var result = new AjaxResponse(wrappedError, isUnauthorized);
        await context.Response.WriteAsJsonAsync(result);
    }

    /// <summary>
    /// Checks if response wrapping should be skipped based on the x-wrap-result header.
    /// </summary>
    private static bool ShouldSkipWrapping(HttpContext context)
    {
        var wrapHeader = context.Request.Headers["x-wrap-result"];
        return bool.FalseString.Equals(wrapHeader, StringComparison.OrdinalIgnoreCase);
    }

    private static System.Net.HttpStatusCode GetStatusCode(Exception exception)
    {
        return exception switch
        {
            AuthorizationException => System.Net.HttpStatusCode.Forbidden,
            EntityNotFoundException => System.Net.HttpStatusCode.NotFound,
            SkywalkerValidationException => System.Net.HttpStatusCode.BadRequest,
            IHasHttpStatusCode hasStatusCode => hasStatusCode.HttpStatusCode,
            _ => System.Net.HttpStatusCode.InternalServerError
        };
    }

    private void LogException(Exception exception)
    {
        var logLevel = exception is IHasLogLevel hasLogLevel
            ? hasLogLevel.LogLevel
            : LogLevel.Error;

        _logger.Log(logLevel, exception, "Exception occurred: {Message}", exception.Message);
    }

    private static async Task NotifyExceptionAsync(HttpContext context, Exception exception)
    {
        var exceptionNotifier = context.RequestServices.GetService<IExceptionNotifier>();
        if (exceptionNotifier == null)
        {
            return;
        }

        await exceptionNotifier.NotifyAsync(new ExceptionNotificationContext(exception));
    }

    private static Task ClearCacheHeaders(object state)
    {
        var response = (HttpResponse)state;
        response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
        response.Headers.Pragma = "no-cache";
        response.Headers.Expires = "0";
        return Task.CompletedTask;
    }
}

