// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace Skywalker.Ddd.AspNetCore.Mvc.Models.Abstractions;

/// <summary>
/// HTTP response DTO that carries a single validation error to the client.
/// </summary>
[Serializable]
public class ValidationErrorDto
{

    /// <summary>
    /// Relate invalid member (fields/properties).
    /// </summary>
    public string Member { get; set; }

    /// <summary>
    /// Validation error message.
    /// </summary>
    public string Message { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    [JsonConstructor]
    private ValidationErrorDto() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    /// <summary>
    /// Creates a new instance of <see cref="ValidationErrorDto"/>.
    /// </summary>
    /// <param name="member">Related invalid member</param>
    /// <param name="message">Validation error message</param>
    public ValidationErrorDto(string member, string message)
    {
        Member = member;
        Message = message;
    }
}
