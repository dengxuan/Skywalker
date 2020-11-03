using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp.Validation;

namespace Skywalker.Validation
{
    public static class HasValidationErrorsExtensions
    {
        public static TException WithValidationError<TException>([NotNull] this TException exception, [NotNull] ValidationResult validationError)
            where TException : class, IHasValidationErrors
        {
            Check.NotNull(exception, nameof(exception));
            Check.NotNull(validationError, nameof(validationError));

            exception.ValidationErrors.Add(validationError);

            return exception;
        }

        public static TException WithValidationError<TException>([NotNull] this TException exception, string errorMessage, params string[] memberNames)
            where TException : class, IHasValidationErrors
        {
            var validationResult = memberNames.IsNullOrEmpty()
                ? new ValidationResult(errorMessage)
                : new ValidationResult(errorMessage, memberNames);

            return exception.WithValidationError(validationResult);
        }
    }
}