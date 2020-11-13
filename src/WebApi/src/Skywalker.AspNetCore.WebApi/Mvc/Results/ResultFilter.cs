using Microsoft.AspNetCore.Mvc.Filters;
using Skywalker.AspNetCore.WebApi.Mvc.Extensions;
using Skywalker.AspNetCore.WebApi.Mvc.Results.Wrapping;
using Skywalker.Reflection;

namespace Skywalker.AspNetCore.WebApi.Abstractions.Mvc.Results
{

    public class ResultFilter : IResultFilter
    {
        private readonly SkywalkerWebApiOptions _options;
        private readonly IActionResultWrapperFactory _actionResultWrapperFactory;

        public ResultFilter(SkywalkerWebApiOptions options,
            IActionResultWrapperFactory actionResultWrapper)
        {
            _options = options;
            _actionResultWrapperFactory = actionResultWrapper;
        }

        public virtual void OnResultExecuting(ResultExecutingContext context)
        {
            var methodInfo = context.ActionDescriptor.GetMethodInfo();

            //var clientCacheAttribute = ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
            //    methodInfo,
            //    _configuration.DefaultClientCacheAttribute
            //);

            //clientCacheAttribute?.Apply(context);

            var wrapResultAttribute = ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(methodInfo, _options.DefaultWrapResultAttribute);

            if (wrapResultAttribute?.WrapOnSuccess != true)
            {
                return;
            }

            _actionResultWrapperFactory.CreateFor(context).Wrap(context);
        }

        public virtual void OnResultExecuted(ResultExecutedContext context)
        {
            //no action
        }
    }
}
