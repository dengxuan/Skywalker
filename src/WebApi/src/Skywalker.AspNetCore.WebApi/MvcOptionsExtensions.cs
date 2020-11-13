using Skywalker.AspNetCore.WebApi.Abstractions.Mvc.Results;
using Skywalker.AspNetCore.WebApi.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Skywalker.AspNetCore.WebApi
{
    internal static class MvcOptionsExtensions
    {
        public static void AddAspNetCore(this MvcOptions options)
        {
            AddFilters(options);
            AddModelBinders(options);
        }

        private static void AddFilters(MvcOptions options)
        {
            options.Filters.AddService(typeof(ExceptionFilter));
            options.Filters.AddService(typeof(ResultFilter));
        }

        private static void AddModelBinders(MvcOptions options)
        {
            //options.ModelBinderProviders.Add(new HermitDateTimeModelBinderProvider());
        }
    }
}
