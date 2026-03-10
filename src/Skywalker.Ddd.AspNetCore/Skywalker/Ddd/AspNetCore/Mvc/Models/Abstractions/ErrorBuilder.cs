// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.AspNetCore.Mvc.Models.Abstractions;

/// <inheritdoc/>
/// <inheritdoc/>
public class ErrorBuilder(ResponseWrapperOptions configuration) : IErrorBuilder
{
    private IExceptionToErrorConverter Converter { get; set; } = new DefaultErrorConverter(configuration);

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
