using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Skywalker.Ddd.AspNetCore.Mvc.Results.Wrapping;

namespace Skywalker.Ddd.AspNetCore.Mvc.Results;


public class ResultFilter(
    ResponseWrapperOptions options,
    IActionResultWrapperFactory actionResultWrapper) : IResultFilter
{
    public virtual void OnResultExecuting(ResultExecutingContext context)
    {
        var wrap = context.HttpContext.Request.Headers["x-wrap-result"];
        if (bool.FalseString.Equals(wrap, StringComparison.InvariantCultureIgnoreCase))
        {
            return;
        }
        var methodInfo = context.ActionDescriptor.GetMethodInfo();
        var wrapResultAttribute = ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(methodInfo, options.DefaultWrapResultAttribute);

        if (wrapResultAttribute?.WrapOnSuccess != true)
        {
            return;
        }
        context.HttpContext.Response.Headers["x-wrap-result"] = bool.TrueString;
        actionResultWrapper.CreateFor(context).Wrap(context);
    }

    public virtual void OnResultExecuted(ResultExecutedContext context)
    {
    }
}
