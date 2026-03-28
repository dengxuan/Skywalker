namespace Skywalker.Exceptions;

/// <summary>
/// Base exception type for those are thrown by Skywalker system for Skywalker specific exceptions.
/// </summary>
[Serializable]
public class SkywalkerException : Exception
{
    public SkywalkerException()
    {
    }

    public SkywalkerException(string? message) : base(message)
    {
    }

    public SkywalkerException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
