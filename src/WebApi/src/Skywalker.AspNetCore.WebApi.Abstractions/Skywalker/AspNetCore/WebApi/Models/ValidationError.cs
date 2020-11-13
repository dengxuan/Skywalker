using System;

namespace Skywalker.AspNetCore.WebApi.Models
{
    /// <summary>
    /// Used to store information about a validation error.
    /// </summary>
    [Serializable]
    public class ValidationError
    {
        /// <summary>
        /// Validation error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Relate invalid members (fields/properties).
        /// </summary>
        public string[] Members { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="ValidationError"/>.
        /// </summary>
        public ValidationError()
        {

        }

        /// <summary>
        /// Creates a new instance of <see cref="ValidationError"/>.
        /// </summary>
        /// <param name="message">Validation error message</param>
        public ValidationError(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ValidationError"/>.
        /// </summary>
        /// <param name="message">Validation error message</param>
        /// <param name="members">Related invalid members</param>
        public ValidationError(string message, string[] members)
            : this(message)
        {
            Members = members;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ValidationError"/>.
        /// </summary>
        /// <param name="message">Validation error message</param>
        /// <param name="member">Related invalid member</param>
        public ValidationError(string message, string member)
            : this(message, new[] { member })
        {

        }
    }
}
