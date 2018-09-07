using Aiursoft.Colossus.Data;
using Aiursoft.Colossus.Models;
using Aiursoft.Colossus.Models.DashboardViewModels;
using Aiursoft.Pylon.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Colossus.Controllers
{
    [AiurForceAuth]
    public class DashboardController : Controller
    {

        private readonly UserManager<ColossusUser> _userManager;
        private readonly ColossusDbContext _dbContext;

        public DashboardController(
            UserManager<ColossusUser> userManager,
            ColossusDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var myFiles = await _dbContext
                .UploadRecords
                .Where(t => t.UploaderId == user.Id)
                .OrderByDescending(t => t.UploadTime)
                .ToListAsync();
#warning Take first items!
            var model = new IndexViewModel(user, 0, "File upload history")
            {
                Files = myFiles
            };
            return View(model);
        }

        private async Task<ColossusUser> GetCurrentUserAsync()
        {
            if (User.Identity.Name == null)
            {
                return null;
            }
            return await _userManager.FindByNameAsync(User.Identity.Name);
        }
    }
}
