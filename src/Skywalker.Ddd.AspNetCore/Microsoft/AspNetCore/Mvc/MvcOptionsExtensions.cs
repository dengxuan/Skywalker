using Skywalker.Ddd.AspNetCore.Mvc.Filters;
using Skywalker.Ddd.AspNetCore.Mvc.Results;

namespace Microsoft.AspNetCore.Mvc;

internal static class MvcOptionsExtensions
{
    internal static void ResponseWrapper(this MvcOptions options)
    {
        AddFilters(options);
    }

    private static void AddFilters(MvcOptions options)
    {
        options.Filters.AddService<ExceptionFilter>();
        options.Filters.AddService<ResultFilter>();
    }
}
