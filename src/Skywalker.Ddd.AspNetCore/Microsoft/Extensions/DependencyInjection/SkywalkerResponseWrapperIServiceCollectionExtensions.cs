using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Skywalker.Ddd.AspNetCore.Mvc;
using Skywalker.Ddd.AspNetCore.Mvc.Filters;
using Skywalker.Ddd.AspNetCore.Mvc.Models.Abstractions;
using Skywalker.Ddd.AspNetCore.Mvc.Results;
using Skywalker.Ddd.AspNetCore.Mvc.Results.Wrapping;

namespace Microsoft.Extensions.DependencyInjection;

public static class SkywalkerResponseWrapperIServiceCollectionExtensions
{

    /// <summary>
    /// Integrates Hermit to AspNet Core.
    /// </summary>
    /// <param name="services">Services.</param>
    /// <param name="optionsAction">An action to get/modify options</param>
    public static IServiceCollection AddResponseWrapper(this IServiceCollection services)
    {
        AddAspNetCoreServices(services);
        return services;
    }

    public static IServiceCollection AddResponseWrapper(this IServiceCollection services, Action<ResponseWrapperOptions> configure)
    {
        services.AddResponseWrapper();
        services.Configure(configure);
        return services;
    }

    private static void AddAspNetCoreServices(IServiceCollection services)
    {
        var builder = services.AddMvcCore(setupAction =>
        {
            setupAction.RespectBrowserAcceptHeader = true;
        });

        services.AddSingleton(c => c.GetRequiredService<IOptions<ResponseWrapperOptions>>().Value);

        services.TryAddSingleton<IErrorBuilder, ErrorBuilder>();

        services.TryAddTransient<ExceptionFilter>();
        services.TryAddTransient<ResultFilter>();

        services.TryAddTransient<IActionResultWrapperFactory, ActionResultWrapperFactory>();

        //See https://github.com/aspnet/Mvc/issues/3936 to know why we added these services.
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.Configure<MvcOptions>(mvcOptions =>
        {
            mvcOptions.ResponseWrapper();
        });
    }
}
