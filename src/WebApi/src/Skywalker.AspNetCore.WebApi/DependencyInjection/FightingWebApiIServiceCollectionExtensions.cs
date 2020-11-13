using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Skywalker.AspNetCore.WebApi.Abstractions.Mvc.Results;
using Skywalker.AspNetCore.WebApi.Models;
using Skywalker.AspNetCore.WebApi.Mvc.Filters;
using Skywalker.AspNetCore.WebApi.Mvc.Results.Wrapping;
using System;

namespace Skywalker.AspNetCore.WebApi.DependencyInjection
{
    public static class FightingWebApiIServiceCollectionExtensions
    {

        /// <summary>
        /// Integrates Hermit to AspNet Core.
        /// </summary>
        /// <param name="services">Services.</param>
        /// <param name="optionsAction">An action to get/modify options</param>
        public static IServiceCollection AddSkywalkerWebApi(this IServiceCollection services)
        {
            AddAspNetCoreServices(services);
            return services;
        }

        public static IServiceCollection AddSkywalkerWebApi(this IServiceCollection services, Action<SkywalkerWebApiOptions> configure)
        {
            services.AddSkywalkerWebApi();
            services.Configure(configure);
            return services;
        }

        private static void AddAspNetCoreServices(IServiceCollection services)
        {
            IMvcCoreBuilder builder = services.AddMvcCore(setupAction =>
            {
                setupAction.RespectBrowserAcceptHeader = true;
            });
            builder.AddApiExplorer();
            builder.AddDataAnnotations();
            builder.AddJsonFormatters();
            builder.AddXmlSerializerFormatters();

            services.AddSingleton(c => c.GetRequiredService<IOptions<SkywalkerWebApiOptions>>().Value);

            services.TryAddSingleton<IErrorBuilder, ErrorBuilder>();
            //services.TryAddSingleton<IAntiforgeryOptions, AntiforgeryOptions>();

            services.TryAddTransient<ExceptionFilter>();
            services.TryAddTransient<ResultFilter>();

            services.TryAddTransient<IActionResultWrapperFactory, ActionResultWrapperFactory>();
            //services.TryAddTransient<IPrincipalAccessor, HermtAspNetCorePrincipalAccessor>();
            //services.TryAddTransient<IAntiforgeryManager, AntiforgeryManager>();

            //See https://github.com/aspnet/Mvc/issues/3936 to know why we added these services.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            //Use DI to create controllers
            //services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

            ////Use DI to create view components
            //services.Replace(ServiceDescriptor.Singleton<IViewComponentActivator, ServiceBasedViewComponentActivator>());

            //Change anti forgery filters (to work proper with non-browser clients)
            //services.Replace(ServiceDescriptor.Transient<AutoValidateAntiforgeryTokenAuthorizationFilter, HermitAutoValidateAntiforgeryTokenAuthorizationFilter>());
            //services.Replace(ServiceDescriptor.Transient<ValidateAntiforgeryTokenAuthorizationFilter, HermitValidateAntiforgeryTokenAuthorizationFilter>());

            //Configure JSON serializer
            //services.Configure<MvcJsonOptions>(jsonOptions =>
            //{
            //    jsonOptions.SerializerSettings.Converters.Insert(0, new DateTimeConverter());
            //});

            //Configure MVC
            services.Configure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.AddAspNetCore();
            });
        }
    }
}
