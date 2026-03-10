namespace Skywalker.Ddd.AspNetCore.ExceptionHandling;

/// <summary>
/// Options for exception handling.
/// </summary>
public class ExceptionHandlingOptions
{
    /// <summary>
    /// Gets or sets whether to send exception details (like stack trace) to clients.
    /// Default is false for security reasons.
    /// </summary>
    public bool SendExceptionDetailsToClients { get; set; }

    /// <summary>
    /// Gets or sets whether to send all exception notifications to subscribers.
    /// Default is true.
    /// </summary>
    public bool SendExceptionsNotifications { get; set; } = true;
}

