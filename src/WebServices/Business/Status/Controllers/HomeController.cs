using Aiursoft.Status.Data;
using Aiursoft.Status.Models.HomeViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Aiursoft.Status.Controllers
{
    public class HomeController : Controller
    {
        private readonly MonitorDataProvider dataProvider;

        public HomeController(MonitorDataProvider dbContext)
        {
            dataProvider = dbContext;
        }

        public IActionResult Index()
        {
            var data = dataProvider.MonitorRules;
            var model = new IndexViewModel
            {
                Data = data
            };
            return View(model);
        }
    }
}
