using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aiursoft.Pylon.Middlewares
{
    public class SwitchLanguageMiddleware
    {
        private readonly RequestDelegate _next;

        public SwitchLanguageMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (string.Equals(context.Request.Path, "/api/setsonlang", StringComparison.OrdinalIgnoreCase))
            {
                var culture = context.Request.Query["culture"];
                var returnUrl = context.Request.Query["returnUrl"];
                context.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddYears(1),
                        SameSite = SameSiteMode.None,
                    });
                context.Response.Redirect(returnUrl);
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}
