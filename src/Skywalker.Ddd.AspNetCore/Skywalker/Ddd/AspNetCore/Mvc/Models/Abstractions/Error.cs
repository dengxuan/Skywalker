// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace Skywalker.Ddd.AspNetCore.Mvc.Models.Abstractions;

/// <summary>
/// Used to store information about an error.
/// </summary>
[Serializable]
public class Error
{
    /// <summary>
    /// Error code.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Error message.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Error details.
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Validation errors if exists.
    /// </summary>
    public ValidationError[]? ValidationErrors { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    [JsonConstructor]
    private Error() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.


    /// <summary>
    /// Creates a new instance of <see cref="Error"/>.
    /// </summary>
    /// <param name="code">Error code</param>
    /// <param name="message">Error message</param>
    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Error"/>.
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="details">Error details</param>
    public Error(string code, string message, string details) : this(code, message)
    {
        Details = details;
    }
}
