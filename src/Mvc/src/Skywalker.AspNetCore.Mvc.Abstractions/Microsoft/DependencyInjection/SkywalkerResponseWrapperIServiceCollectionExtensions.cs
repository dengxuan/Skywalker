using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Skywalker.AspNetCore.Mvc;
using Skywalker.AspNetCore.Mvc.Filters;
using Skywalker.AspNetCore.Mvc.Models;
using Skywalker.AspNetCore.Mvc.Response.Wrapping;
using Skywalker.AspNetCore.Mvc.Results;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SkywalkerResponseWrapperIServiceCollectionExtensions
    {

        /// <summary>
        /// Integrates Hermit to AspNet Core.
        /// </summary>
        /// <param name="services">Services.</param>
        /// <param name="optionsAction">An action to get/modify options</param>
        public static IServiceCollection AddWebApiResponseWrapper(this IServiceCollection services)
        {
            AddAspNetCoreServices(services);
            return services;
        }

        public static IServiceCollection AddWebApiResponseWrapper(this IServiceCollection services, Action<SkywalkerResponseWrapperOptions> configure)
        {
            services.AddWebApiResponseWrapper();
            services.Configure(configure);
            return services;
        }

        private static void AddAspNetCoreServices(IServiceCollection services)
        {
            IMvcCoreBuilder builder = services.AddMvcCore(setupAction =>
            {
                setupAction.RespectBrowserAcceptHeader = true;
            });

            services.AddSingleton(c => c.GetRequiredService<IOptions<SkywalkerResponseWrapperOptions>>().Value);

            services.TryAddSingleton<IErrorBuilder, ErrorBuilder>();

            services.TryAddTransient<ExceptionFilter>();
            services.TryAddTransient<ResultFilter>();

            services.TryAddTransient<IActionResultWrapperFactory, ActionResultWrapperFactory>();

            //See https://github.com/aspnet/Mvc/issues/3936 to know why we added these services.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.Configure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.ResponseWrapper();
            });
        }
    }
}
