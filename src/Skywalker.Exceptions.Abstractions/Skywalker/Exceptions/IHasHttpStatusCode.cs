using System.Net;

namespace Skywalker.Exceptions;

public interface IHasHttpStatusCode
{
    HttpStatusCode HttpStatusCode { get; }
}
