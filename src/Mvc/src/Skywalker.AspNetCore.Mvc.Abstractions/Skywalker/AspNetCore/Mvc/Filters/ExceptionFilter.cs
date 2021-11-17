using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Skywalker.AspNetCore.Mvc.Abstractions;
using Skywalker.AspNetCore.Mvc.Abstractions.Models;
using Skywalker.AspNetCore.Mvc.Models;
using Skywalker.AspNetCore.Mvc.Response;
using Skywalker.Reflection;
using System.Net;

namespace Skywalker.AspNetCore.Mvc.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;
        private readonly IErrorBuilder _errorBuilder;
        private readonly SkywalkerResponseWrapperOptions _options;

        public ExceptionFilter(IErrorBuilder errorBuilder, SkywalkerResponseWrapperOptions options, ILoggerFactory loggerFactory)
        {
            _errorBuilder = errorBuilder;
            _options = options;
            _logger = loggerFactory.CreateLogger<ExceptionFilter>();
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

            context.Result = new ObjectResult(new AjaxResponse(_errorBuilder.BuildForException(context.Exception)));

            /* Handled! */
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Exception = null;
        }
    }
}
