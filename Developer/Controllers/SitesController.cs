using Aiursoft.Developer.Data;
using Aiursoft.Developer.Models.SitesViewModels;
using Aiursoft.Pylon.Models.Developer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<IActionResult> CreateSite(string id)// app id
        {
            var user = await GetCurrentUserAsync();
            var model = new CreateSiteViewModel(user)
            {
                AppId = id
            };
            return View(model);
        }

        private async Task<DeveloperUser> GetCurrentUserAsync()
        {
            return await _dbContext.Users.Include(t => t.MyApps).SingleOrDefaultAsync(t => t.UserName == User.Identity.Name);
        }
    }
}
