using Microsoft.AspNetCore.RequestLocalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skywalker;
using Skywalker.AspNetCore.Tracing;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.AspNetCore.Builder
{
    public static class SkywalkerApplicationBuilderExtensions
    {
        public static void InitializeApplication([NotNull] this IApplicationBuilder app)
        {
            Check.NotNull(app, nameof(app));

            app.ApplicationServices.GetRequiredService<ObjectAccessor<IApplicationBuilder>>().Value = app;
            var applicationLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            applicationLifetime.ApplicationStopping.Register(() =>
            {
            });

            applicationLifetime.ApplicationStopped.Register(() =>
            {
            });

        }

        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        {
            return app
                .UseMiddleware<SkywalkerCorrelationIdMiddleware>();
        }

        public static IApplicationBuilder UseSkywalkerRequestLocalization(this IApplicationBuilder app,
            Action<RequestLocalizationOptions> optionsAction = null)
        {
            app.ApplicationServices
                .GetRequiredService<ISkywalkerRequestLocalizationOptionsProvider>()
                .InitLocalizationOptions(optionsAction);

            return app.UseMiddleware<SkywalkerRequestLocalizationMiddleware>();
        }
    }
}
