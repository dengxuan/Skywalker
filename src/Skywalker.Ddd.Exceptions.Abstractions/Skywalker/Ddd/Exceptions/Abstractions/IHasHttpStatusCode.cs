using System.Net;

namespace Skywalker.Ddd.Exceptions;

/// <summary>
/// Interface for exceptions that have a specific HTTP status code.
/// </summary>
public interface IHasHttpStatusCode
{
    /// <summary>
    /// Gets the HTTP status code for this exception.
    /// </summary>
    HttpStatusCode HttpStatusCode { get; }
}

