namespace Skywalker.Ddd.Exceptions;

/// <summary>
/// Represents error information returned to client.
/// </summary>
public class ErrorInfo
{
    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the detailed error message.
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Gets or sets the validation errors.
    /// </summary>
    public ValidationErrorInfo[]? ValidationErrors { get; set; }

    /// <summary>
    /// Creates a new <see cref="ErrorInfo"/> object.
    /// </summary>
    public ErrorInfo()
    {
    }

    /// <summary>
    /// Creates a new <see cref="ErrorInfo"/> object.
    /// </summary>
    /// <param name="message">Error message</param>
    public ErrorInfo(string message)
    {
        Message = message;
    }

    /// <summary>
    /// Creates a new <see cref="ErrorInfo"/> object.
    /// </summary>
    /// <param name="code">Error code</param>
    /// <param name="message">Error message</param>
    public ErrorInfo(string? code, string message)
    {
        Code = code;
        Message = message;
    }

    /// <summary>
    /// Creates a new <see cref="ErrorInfo"/> object.
    /// </summary>
    /// <param name="code">Error code</param>
    /// <param name="message">Error message</param>
    /// <param name="details">Error details</param>
    public ErrorInfo(string? code, string message, string? details)
    {
        Code = code;
        Message = message;
        Details = details;
    }
}

/// <summary>
/// Represents a validation error.
/// </summary>
public class ValidationErrorInfo
{
    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the members related to this error.
    /// </summary>
    public string[]? Members { get; set; }

    /// <summary>
    /// Creates a new <see cref="ValidationErrorInfo"/> object.
    /// </summary>
    public ValidationErrorInfo()
    {
    }

    /// <summary>
    /// Creates a new <see cref="ValidationErrorInfo"/> object.
    /// </summary>
    /// <param name="message">Error message</param>
    public ValidationErrorInfo(string message)
    {
        Message = message;
    }

    /// <summary>
    /// Creates a new <see cref="ValidationErrorInfo"/> object.
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="members">Related members</param>
    public ValidationErrorInfo(string message, string[] members)
    {
        Message = message;
        Members = members;
    }
}

