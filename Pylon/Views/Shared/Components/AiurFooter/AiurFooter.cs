using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Pylon.Views.Shared.Components.AiurFooter
{
    public class AiurFooter : ViewComponent
    {
        public IViewComponentResult Invoke(string itemClass, string template)
        {
            var model = new AiurFooterViewModel
            {
                Class = itemClass,
                Template = template
            };
            return View(model);
        }
    }
}
