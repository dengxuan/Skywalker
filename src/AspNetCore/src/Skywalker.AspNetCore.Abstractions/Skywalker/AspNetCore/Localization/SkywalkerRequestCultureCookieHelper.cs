﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;

namespace Skywalker.AspNetCore.Localization
{
    public static class SkywalkerRequestCultureCookieHelper
    {
        public static void SetCultureCookie(
            HttpContext httpContext,
            RequestCulture requestCulture)
        {
            httpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(requestCulture),
                new CookieOptions
                {
                    Expires = DateTime.Now.AddYears(2)
                }
            );
        }
    }
}
