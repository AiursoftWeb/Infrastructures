using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.SDK.Views.Shared.Components.ChinaRegister
{
    public class ChinaRegister : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var requestCulture = requestCultureFeature.RequestCulture.UICulture.IetfLanguageTag;
            var model = new ChinaRegisterViewModel
            {
                IsInChina = requestCulture.ToLower().StartsWith("zh")
            };
            return View(model);
        }
    }
}
