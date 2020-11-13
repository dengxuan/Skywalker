using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Skywalker.AspNetCore.WebApi.Models;
using Skywalker.AspNetCore.WebApi.Mvc.Extensions;
using Skywalker.AspNetCore.WebApi.Mvc.Results;
using Skywalker.Reflection;
using System.Net;

namespace Skywalker.AspNetCore.WebApi.Mvc.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;
        private readonly IErrorBuilder _errorBuilder;
        private readonly SkywalkerWebApiOptions _options;

        public ExceptionFilter(IErrorBuilder errorBuilder, SkywalkerWebApiOptions options, ILoggerFactory loggerFactory)
        {
            _errorBuilder = errorBuilder;
            _options = options;
            _logger = (loggerFactory ?? NullLoggerFactory.Instance).CreateLogger<ExceptionFilter>();
        }

        public void OnException(ExceptionContext context)
        {
            var wrapResultAttribute = ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(context.ActionDescriptor.GetMethodInfo(), _options.DefaultWrapResultAttribute);

            if (wrapResultAttribute!.LogError)
            {
                _logger.LogError(context.Exception, context.Exception.Message);
            }

            if (wrapResultAttribute.WrapOnError)
            {
                HandleAndWrapException(context);
            }
        }

        private void HandleAndWrapException(ExceptionContext context)
        {
            if (!ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
            {
                return;
            }

            context.HttpContext.Response.StatusCode = GetStatusCode(context);

            context.Result = new ObjectResult(new AjaxResponse(_errorBuilder.BuildForException(context.Exception)));

            context.Exception = null; //Handled!
        }

        private int GetStatusCode(ExceptionContext context)
        {
            //if (context.Exception is AuthorizationException)
            //{
            //    return context.HttpContext.User.Identity.IsAuthenticated
            //        ? (int)HttpStatusCode.Forbidden
            //        : (int)HttpStatusCode.Unauthorized;
            //}

            //if (context.Exception is HermitValidationException)
            //{
            //    return (int)HttpStatusCode.BadRequest;
            //}

            //if (context.Exception is EntityNotFoundException)
            //{
            //    return (int)HttpStatusCode.NotFound;
            //}

            return (int)HttpStatusCode.InternalServerError;
        }
    }
}
