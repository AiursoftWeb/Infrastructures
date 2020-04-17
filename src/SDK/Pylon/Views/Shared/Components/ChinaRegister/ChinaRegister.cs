using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Pylon.Views.Shared.Components.ChinaRegister
{
    public class ChinaRegister : ViewComponent
    {
        public ChinaRegister()
        {

        }

        public IViewComponentResult Invoke()
        {
            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var requestCulture = requestCultureFeature.RequestCulture.UICulture.IetfLanguageTag;
            var model = new ChinaRegisterViewModel
            {
                IsInChina = requestCulture == "zh"
            };
            return View(model);
        }
    }
}
