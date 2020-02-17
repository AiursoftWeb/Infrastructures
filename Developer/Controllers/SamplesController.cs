using Aiursoft.Developer.Models.SamplesViewModels;
using Aiursoft.Handler.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Controllers
{
    [LimitPerMin]
    public class SamplesController : Controller
    {
        public ActionResult DisableWithForm()
        {
            var model = new DisableWithFormViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableWithFormSubmit()
        {
            await Task.Delay(4000);
            return RedirectToAction(nameof(DisableWithForm));
        }

        public IActionResult UTCTime()
        {
            return View();
        }

        public IActionResult Scanner()
        {
            return View();
        }
    }
}
