﻿using Microsoft.Extensions.Logging;
using Skywalker;
using Skywalker.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace Volo.Abp.Validation
{
    /// <summary>
    /// This exception type is used to throws validation exceptions.
    /// </summary>
    [Serializable]
    public class SkywalkerValidationException : SkywalkerException, 
        IHasLogLevel, 
        IHasValidationErrors, 
        IExceptionWithSelfLogging
    {
        /// <summary>
        /// Detailed list of validation errors for this exception.
        /// </summary>
        public IList<ValidationResult> ValidationErrors { get; }

        /// <summary>
        /// Exception severity.
        /// Default: Warn.
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SkywalkerValidationException()
        {
            ValidationErrors = new List<ValidationResult>();
            LogLevel = LogLevel.Warning;
        }

        /// <summary>
        /// Constructor for serializing.
        /// </summary>
        public SkywalkerValidationException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
            ValidationErrors = new List<ValidationResult>();
            LogLevel = LogLevel.Warning;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public SkywalkerValidationException(string message)
            : base(message)
        {
            ValidationErrors = new List<ValidationResult>();
            LogLevel = LogLevel.Warning;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="validationErrors">Validation errors</param>
        public SkywalkerValidationException(IList<ValidationResult> validationErrors)
        {
            ValidationErrors = validationErrors;
            LogLevel = LogLevel.Warning;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="validationErrors">Validation errors</param>
        public SkywalkerValidationException(string message, IList<ValidationResult> validationErrors)
            : base(message)
        {
            ValidationErrors = validationErrors;
            LogLevel = LogLevel.Warning;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public SkywalkerValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
            ValidationErrors = new List<ValidationResult>();
            LogLevel = LogLevel.Warning;
        }

        public void Log(ILogger logger)
        {
            if (ValidationErrors.IsNullOrEmpty())
            {
                return;
            }

            logger.Log(LogLevel, "There are " + ValidationErrors.Count + " validation errors:");
            foreach (var validationResult in ValidationErrors)
            {
                var memberNames = "";
                if (validationResult.MemberNames != null && validationResult.MemberNames.Any())
                {
                    memberNames = " (" + string.Join(", ", validationResult.MemberNames) + ")";
                }

                logger.Log(LogLevel, validationResult.ErrorMessage + memberNames);
            }
        }
    }
}
