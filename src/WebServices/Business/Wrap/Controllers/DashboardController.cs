using Aiursoft.Handler.Attributes;
using Aiursoft.Identity.Attributes;
using Aiursoft.Wrap.Models;
using Aiursoft.Wrap.Models.DashboardViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Wrap.Controllers
{
    [LimitPerMin]
    [AiurForceAuth]
    public class DashboardController : Controller
    {
        private readonly UserManager<WrapUser> _userManager;

        public DashboardController(UserManager<WrapUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var model = new IndexViewModel(user);
            return View(model);
        }

        private async Task<WrapUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }
    }
}
