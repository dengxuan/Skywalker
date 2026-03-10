using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Skywalker.Ddd.AspNetCore.Mvc.Models.Abstractions;
using Skywalker.Ddd.AspNetCore.Mvc.Results;

namespace Skywalker.Ddd.AspNetCore.Mvc.Filters;

public class ExceptionFilter(IErrorBuilder errorBuilder, ResponseWrapperOptions options, ILogger<ExceptionFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var wrapResultAttribute = ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(context.ActionDescriptor.GetMethodInfo(), options.DefaultWrapResultAttribute);

        if (wrapResultAttribute?.LogError == true)
        {
            logger.LogError(context.Exception, "{message}", context.Exception.Message);
        }

        if (wrapResultAttribute?.WrapOnError == true)
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

        context.Result = new ObjectResult(new AjaxResponse(errorBuilder.BuildForException(context.Exception)));

        context.HttpContext.Response.Headers["x-wrap-result"] = bool.TrueString;
        /* Handled! */
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
    }
}
