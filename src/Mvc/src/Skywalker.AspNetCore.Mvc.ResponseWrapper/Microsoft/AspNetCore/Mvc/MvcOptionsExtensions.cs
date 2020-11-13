using Skywalker.AspNetCore.Mvc.Filters;
using Skywalker.AspNetCore.Mvc.Results;

namespace Microsoft.AspNetCore.Mvc
{
    internal static class MvcOptionsExtensions
    {
        internal static void ResponseWrapper(this MvcOptions options)
        {
            AddFilters(options);
        }

        private static void AddFilters(MvcOptions options)
        {
            options.Filters.AddService(typeof(ExceptionFilter));
            options.Filters.AddService(typeof(ResultFilter));
        }
    }
}
