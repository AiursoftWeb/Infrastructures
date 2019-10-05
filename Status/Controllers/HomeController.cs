using Aiursoft.Status.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Aiursoft.Status.Controllers
{
    public class HomeController : Controller
    {
        private readonly StatusDbContext _dbContext;

        public HomeController(StatusDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _dbContext.MonitorRules.ToListAsync();
            return Json(data);
        }
    }
}
