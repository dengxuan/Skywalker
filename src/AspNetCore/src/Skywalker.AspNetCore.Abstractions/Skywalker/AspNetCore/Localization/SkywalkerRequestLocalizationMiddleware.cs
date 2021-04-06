using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.RequestLocalization
{
    public class SkywalkerRequestLocalizationMiddleware : IMiddleware
    {
        private readonly ISkywalkerRequestLocalizationOptionsProvider _requestLocalizationOptionsProvider;
        private readonly ILoggerFactory _loggerFactory;

        public SkywalkerRequestLocalizationMiddleware(
            ISkywalkerRequestLocalizationOptionsProvider requestLocalizationOptionsProvider,
            ILoggerFactory loggerFactory)
        {
            _requestLocalizationOptionsProvider = requestLocalizationOptionsProvider;
            _loggerFactory = loggerFactory;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var middleware = new RequestLocalizationMiddleware(
                next,
                new OptionsWrapper<RequestLocalizationOptions>(await _requestLocalizationOptionsProvider.GetLocalizationOptionsAsync()),
                _loggerFactory
            );

            await middleware.Invoke(context);
        }
    }
}
