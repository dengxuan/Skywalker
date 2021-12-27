using System.Runtime.Serialization;

namespace Skywalker.Extensions.Exceptions;

/// <summary>
/// Base exception type for those are thrown by Skywalker system for Skywalker specific exceptions.
/// </summary>
[Serializable]
public class SkywalkerException : Exception
{
    /// <summary>
    /// Creates a new <see cref="SkywalkerException"/> object.
    /// </summary>
    public SkywalkerException()
    {

    }

    /// <summary>
    /// Creates a new <see cref="SkywalkerException"/> object.
    /// </summary>
    public SkywalkerException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
    {

    }

    /// <summary>
    /// Creates a new <see cref="SkywalkerException"/> object.
    /// </summary>
    /// <param name="message">Exception message</param>
    public SkywalkerException(string message) : base(message)
    {

    }

    /// <summary>
    /// Creates a new <see cref="SkywalkerException"/> object.
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exception</param>
    public SkywalkerException(string message, Exception? innerException) : base(message, innerException)
    {

    }
}
