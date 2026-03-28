using System.Net;

namespace Skywalker.Exceptions;

public interface IHttpExceptionStatusCodeFinder
{
    HttpStatusCode GetStatusCode(object httpContext, Exception exception);
}
