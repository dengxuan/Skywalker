using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Skywalker.AspNetCore.Mvc.Abstractions;
using Skywalker.AspNetCore.Mvc.Response.Wrapping;
using Skywalker.Reflection;

namespace Skywalker.AspNetCore.Mvc.Response
{

    public class ResultFilter : IResultFilter
    {
        private readonly SkywalkerResponseWrapperOptions _options;
        private readonly IActionResultWrapperFactory _actionResultWrapperFactory;

        public ResultFilter(SkywalkerResponseWrapperOptions options,
            IActionResultWrapperFactory actionResultWrapper)
        {
            _options = options;
            _actionResultWrapperFactory = actionResultWrapper;
        }

        public virtual void OnResultExecuting(ResultExecutingContext context)
        {
            var methodInfo = context.ActionDescriptor.GetMethodInfo();

            var wrapResultAttribute = ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(methodInfo, _options.DefaultWrapResultAttribute);

            if (wrapResultAttribute?.WrapOnSuccess != true)
            {
                return;
            }

            _actionResultWrapperFactory.CreateFor(context).Wrap(context);
        }

        public virtual void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}
