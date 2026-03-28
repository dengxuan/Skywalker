using Skywalker.Exceptions;

namespace Skywalker.Ddd.AspNetCore.ExceptionHandling;

/// <summary>
/// Default implementation of <see cref="IExceptionToErrorInfoConverter"/>.
/// </summary>
public class DefaultExceptionToErrorInfoConverter : IExceptionToErrorInfoConverter
{
    /// <inheritdoc />
    public virtual ErrorInfo Convert(Exception exception, ExceptionConvertOptions? options = null)
    {
        options ??= new ExceptionConvertOptions();

        var errorInfo = CreateErrorInfoWithoutCode(exception, options);

        if (exception is IHasErrorCode hasErrorCode)
        {
            errorInfo.Code = hasErrorCode.Code;
        }

        return errorInfo;
    }

    /// <summary>
    /// Creates error info without error code.
    /// </summary>
    protected virtual ErrorInfo CreateErrorInfoWithoutCode(Exception exception, ExceptionConvertOptions options)
    {
        if (exception is UserFriendlyException userFriendlyException)
        {
            return new ErrorInfo(userFriendlyException.Code, userFriendlyException.Message);
        }

        if (exception is SkywalkerValidationException validationException)
        {
            return CreateValidationErrorInfo(validationException);
        }

        if (exception is EntityNotFoundException entityNotFoundException)
        {
            return new ErrorInfo(
                "Skywalker:EntityNotFound",
                entityNotFoundException.Message);
        }

        if (exception is AuthorizationException authorizationException)
        {
            return new ErrorInfo(
                authorizationException.Code,
                authorizationException.Message);
        }

        // For other exceptions
        var message = options.IncludeDetails
            ? exception.Message
            : "An internal error occurred. Please try again later.";

        var errorInfo = new ErrorInfo(message);

        if (options.IncludeDetails)
        {
            errorInfo.Details = exception.ToString();
        }

        return errorInfo;
    }

    /// <summary>
    /// Creates error info for validation exceptions.
    /// </summary>
    protected virtual ErrorInfo CreateValidationErrorInfo(SkywalkerValidationException exception)
    {
        var errorInfo = new ErrorInfo("Skywalker:Validation", exception.Message);

        if (exception.ValidationErrors.Count > 0)
        {
            errorInfo.ValidationErrors = exception.ValidationErrors
                .Select(e => new ValidationErrorInfo(
                    e.ErrorMessage ?? string.Empty,
                    e.MemberNames.ToArray()))
                .ToArray();
        }

        return errorInfo;
    }
}

