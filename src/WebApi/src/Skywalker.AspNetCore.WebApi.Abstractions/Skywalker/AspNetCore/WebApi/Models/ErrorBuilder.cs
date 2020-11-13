using System;

namespace Skywalker.AspNetCore.WebApi.Models
{
    /// <inheritdoc/>
    public class ErrorBuilder : IErrorBuilder
    {
        private IExceptionToErrorConverter Converter { get; set; }

        /// <inheritdoc/>
        public ErrorBuilder(SkywalkerWebApiOptions configuration)
        {
            Converter = new DefaultErrorConverter(configuration);
        }

        /// <inheritdoc/>
        public Error BuildForException(Exception exception)
        {
            return Converter.Convert(exception);
        }

        /// <summary>
        /// Adds an exception converter that is used by <see cref="BuildForException"/> method.
        /// </summary>
        /// <param name="converter">Converter object</param>
        public void AddExceptionConverter(IExceptionToErrorConverter converter)
        {
            converter.Next = Converter;
            Converter = converter;
        }
    }
}
