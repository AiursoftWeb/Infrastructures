using Aiursoft.Developer.Data;
using Aiursoft.Developer.Models.SitesViewModels;
using Aiursoft.Pylon.Models.Developer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Aiursoft.Developer.Controllers
{
    public class SitesController : Controller
    {
        public DeveloperDbContext _dbContext;
        public SitesController(DeveloperDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var model = new IndexViewModel(user);
            return View(model);
        }

        private async Task<DeveloperUser> GetCurrentUserAsync()
        {
            return await _dbContext.Users.Include(t => t.MyApps).SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
        }
    }
}
