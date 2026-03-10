using System.Net;
using Skywalker.Ddd.Exceptions;

namespace Skywalker.Ddd.AspNetCore.ExceptionHandling;

/// <summary>
/// Default implementation of <see cref="IHttpExceptionStatusCodeFinder"/>.
/// Maps common exception types to appropriate HTTP status codes.
/// </summary>
public class DefaultHttpExceptionStatusCodeFinder : IHttpExceptionStatusCodeFinder
{
    /// <inheritdoc />
    public virtual HttpStatusCode GetStatusCode(object httpContext, Exception exception)
    {
        if (exception is IHasHttpStatusCode hasHttpStatusCode)
        {
            return hasHttpStatusCode.HttpStatusCode;
        }

        return exception switch
        {
            AuthorizationException => HttpStatusCode.Forbidden,
            EntityNotFoundException => HttpStatusCode.NotFound,
            SkywalkerValidationException => HttpStatusCode.BadRequest,
            UserFriendlyException => HttpStatusCode.BadRequest,
            NotImplementedException => HttpStatusCode.NotImplemented,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError
        };
    }
}

