using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RequestLocalization;
using Skywalker;
using Skywalker.Localization;
using System;

namespace Volo.Abp.AspNetCore.Mvc.Localization
{
    [Area("Abp")]
    [Route("Abp/Languages/[action]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AbpLanguagesController : AbpController
    {
        [HttpGet]
        public IActionResult Switch(string culture, string uiCulture = "", string returnUrl = "")
        {
            if (!CultureHelper.IsValidCultureCode(culture))
            {
                throw new SkywalkerException("Unknown language: " + culture + ". It must be a valid culture!");
            }

            AbpRequestCultureCookieHelper.SetCultureCookie(
                HttpContext,
                new RequestCulture(culture, uiCulture)
            );

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(GetRedirectUrl(returnUrl));
            }

            return Redirect("~/");
        }

        private string GetRedirectUrl(string returnUrl)
        {
            if (returnUrl.IsNullOrEmpty())
            {
                return "~/";
            }

            if (Url.IsLocalUrl(returnUrl))
            {
                return returnUrl;
            }

            return "~/";
        }
    }
}
