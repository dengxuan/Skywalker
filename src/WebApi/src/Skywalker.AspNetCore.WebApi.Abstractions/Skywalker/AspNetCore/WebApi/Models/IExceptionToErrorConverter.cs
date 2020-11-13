using System;

namespace Skywalker.AspNetCore.WebApi.Models
{
    /// <summary>
    /// This interface can be implemented to convert an <see cref="Exception"/> object to an <see cref="ErrorInfo"/> object. 
    /// Implements Chain Of Responsibility pattern.
    /// </summary>
    public interface IExceptionToErrorConverter
    {
        /// <summary>
        /// Next converter. If this converter decide this exception is not known, it can call Next.Convert(...).
        /// </summary>
        IExceptionToErrorConverter Next { set; }

        /// <summary>
        /// Converter method.
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <returns>Error info or null</returns>
        Error Convert(Exception exception);
    }
}
