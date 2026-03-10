namespace Skywalker.Ddd.Data;

public class SkywalkerDbConcurrencyException : Exception
{
    /// <summary>
    /// Creates a new <see cref="SkywalkerDbConcurrencyException"/> object.
    /// </summary>
    public SkywalkerDbConcurrencyException()
    {

    }

    /// <summary>
    /// Creates a new <see cref="SkywalkerDbConcurrencyException"/> object.
    /// </summary>
    /// <param name="message">Exception message</param>
    public SkywalkerDbConcurrencyException(string message)
        : base(message)
    {

    }

    /// <summary>
    /// Creates a new <see cref="SkywalkerDbConcurrencyException"/> object.
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Inner exception</param>
    public SkywalkerDbConcurrencyException(string message, Exception innerException)
        : base(message, innerException)
    {

    }
}
