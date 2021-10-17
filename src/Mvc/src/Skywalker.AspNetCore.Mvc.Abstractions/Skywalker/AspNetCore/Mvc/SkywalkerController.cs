using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Skywalker.Application.Abstractions;
using Skywalker.AspNetCore.Mvc.Models;
using Skywalker.Localization;
using Skywalker.Localization.Resources.SkywalkerLocalization;
using System;
using System.Collections.Generic;

namespace Skywalker.AspNetCore.Mvc
{
    [Route("api/[controller]")]
    [ApiController]
    [WrapResult]
    public abstract class SkywalkerController : ControllerBase
    {
        protected IApplication Application => HttpContext.RequestServices.GetRequiredService<IApplication>();

        protected IStringLocalizerFactory StringLocalizerFactory => HttpContext.RequestServices.GetRequiredService<IStringLocalizerFactory>();

        protected IStringLocalizer L
        {
            get
            {
                if (_localizer == null)
                {
                    _localizer = CreateLocalizer();
                }

                return _localizer;
            }
        }
        private IStringLocalizer? _localizer;

        protected Type LocalizationResource
        {
            get => _localizationResource;
            set
            {
                _localizationResource = value;
                _localizer = null;
            }
        }
        private Type _localizationResource = typeof(SkywalkerLocalizationResource);

        protected virtual IStringLocalizer CreateLocalizer()
        {
            if (LocalizationResource != null)
            {
                return StringLocalizerFactory.Create(LocalizationResource);
            }

            var localizer = StringLocalizerFactory.CreateDefaultOrNull();
            if (localizer == null)
            {
                throw new SkywalkerException($"Set {nameof(LocalizationResource)} or define the default localization resource type (by configuring the {nameof(SkywalkerLocalizationOptions)}.{nameof(SkywalkerLocalizationOptions.DefaultResourceType)}) to be able to use the {nameof(L)} object!");
            }

            return localizer;
        }

        protected virtual RedirectResult RedirectSafely(string returnUrl, string? returnUrlHash = null)
        {
            return Redirect(GetRedirectUrl(returnUrl, returnUrlHash));
        }

        protected virtual string? GetRedirectUrl(string? returnUrl, string? returnUrlHash = null)
        {
            returnUrl = NormalizeReturnUrl(returnUrl);

            if (!returnUrlHash.IsNullOrWhiteSpace())
            {
                returnUrl += returnUrlHash;
            }

            return returnUrl;
        }

        protected virtual string? NormalizeReturnUrl(string? returnUrl)
        {
            if (returnUrl.IsNullOrEmpty())
            {
                return GetAppHomeUrl();
            }

            if (Url.IsLocalUrl(returnUrl) /*|| AppUrlProvider.IsRedirectAllowedUrl(returnUrl)*/)
            {
                return returnUrl;
            }

            return GetAppHomeUrl();
        }

        protected virtual string? GetAppHomeUrl()
        {
            return Url.Content("~/");
        }
    }
}
