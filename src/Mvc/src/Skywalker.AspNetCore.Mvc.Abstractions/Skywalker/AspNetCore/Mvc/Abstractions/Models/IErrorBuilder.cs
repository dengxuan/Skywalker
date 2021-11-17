using System;

namespace Skywalker.AspNetCore.Mvc.Abstractions.Models
{
    /// <summary>
    /// This interface is used to build <see cref="ErrorInformation"/> objects.
    /// </summary>
    public interface IErrorBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="ErrorInformation"/> using the given <paramref name="exception"/> object.
        /// </summary>
        /// <param name="exception">The exception object</param>
        /// <returns>Created <see cref="ErrorInformation"/> object</returns>
        Error BuildForException(Exception exception);

        /// <summary>
        /// Adds an <see cref="IExceptionToErrorConverter"/> object.
        /// </summary>
        /// <param name="converter">Converter</param>
        void AddExceptionConverter(IExceptionToErrorConverter converter);
    }
}
