using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Skywalker.AspNetCore.Localization;
using Skywalker.Ddd.ExceptionHandling;
using Skywalker.Localization;

namespace Skywalker.AspNetCore.Mvc.Localization
{
    [Area("skywalker")]
    [Route("skywalker/[controller]")]
    public class LanguagesController : SkywalkerController
    {

        [HttpPut]
        [Route("switch")]
        public IActionResult Switch(string culture, string uiCulture = "", string returnUrl = "")
        {
            if (!CultureHelper.IsValidCultureCode(culture))
            {
                throw new SkywalkerException("Unknown language: " + culture + ". It must be a valid culture!");
            }

            SkywalkerRequestCultureCookieHelper.SetCultureCookie(
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
