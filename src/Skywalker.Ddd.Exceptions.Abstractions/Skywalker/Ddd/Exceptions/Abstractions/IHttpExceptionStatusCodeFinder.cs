using System.Net;

namespace Skywalker.Ddd.Exceptions;

/// <summary>
/// Interface to find HTTP status code for exceptions.
/// </summary>
public interface IHttpExceptionStatusCodeFinder
{
    /// <summary>
    /// Gets the HTTP status code for the given exception.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <param name="exception">The exception.</param>
    /// <returns>HTTP status code.</returns>
    HttpStatusCode GetStatusCode(object httpContext, Exception exception);
}

