namespace Skywalker.Ddd.AspNetCore.Mvc.Models.Abstractions;

public abstract class AjaxResponseBase
{
    /// <summary>
    /// Indicates success status of the result.
    /// Set <see cref="Error"/> if this value is false.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error details (Must and only set if <see cref="Success"/> is false).
    /// </summary>
    public Error? Error { get; set; }

    /// <summary>
    /// This property can be used to indicate that the current user has no privilege to perform this request.
    /// </summary>
    public bool UnAuthorizedRequest { get; set; }
}
