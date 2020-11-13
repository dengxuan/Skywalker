using System;
using System.Collections.Generic;
using System.Text;

namespace Skywalker.AspNetCore.WebApi.Models
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
        public string Details { get; set; }

        /// <summary>
        /// Validation errors if exists.
        /// </summary>
        public ValidationError[] ValidationErrors { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="Error"/>.
        /// </summary>
        public Error()
        {

        }

        /// <summary>
        /// Creates a new instance of <see cref="Error"/>.
        /// </summary>
        /// <param name="message">Error message</param>
        public Error(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Error"/>.
        /// </summary>
        /// <param name="code">Error code</param>
        public Error(int code)
        {
            Code = code;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Error"/>.
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error message</param>
        public Error(int code, string message)
            : this(message)
        {
            Code = code;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Error"/>.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="details">Error details</param>
        public Error(string message, string details)
            : this(message)
        {
            Details = details;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Error"/>.
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error message</param>
        /// <param name="details">Error details</param>
        public Error(int code, string message, string details)
            : this(message, details)
        {
            Code = code;
        }
    }
}
