using System;

namespace Skywalker.AspNetCore.Mvc.Models
{
    /// <summary>
    /// Used to store information about an error.
    /// </summary>
    [Serializable]
    public class Error
    {
        /// <summary>
        /// Error code.
        /// </summary>
        public int Code { get; set; }

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

        /// <summary>
        /// Creates a new instance of <see cref="Error"/>.
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error message</param>
        public Error(int code, string message)
        {
            Code = code;
            Message = message;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Error"/>.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="details">Error details</param>
        public Error(int code, string message, string details) : this(code, message)
        {
            Details = details;
        }
    }
}
