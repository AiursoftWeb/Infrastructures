using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.EE.Models;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Aiursoft.Pylon.Models;
using Aiursoft.EE.Data;
using Microsoft.EntityFrameworkCore;
using Aiursoft.Pylon.Services;
using Aiursoft.EE.Models.ChapterViewModels;

namespace Aiursoft.EE.Controllers
{
    public class ChapterController : Controller
    {
        public readonly SignInManager<EEUser> _signInManager;
        public readonly ILogger _logger;
        public readonly EEDbContext _dbContext;
        private readonly ServiceLocation _serviceLocation;

        public ChapterController(
            SignInManager<EEUser> signInManager,
            ILoggerFactory loggerFactory,
            EEDbContext dbContext,
            ServiceLocation serviceLocation)
        {
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<HomeController>();
            _dbContext = dbContext;
            _serviceLocation = serviceLocation;
        }

        [HttpGet]
        [AiurForceAuth]
        public async Task<IActionResult> Create(int id)//Course Id
        {
            var course = await _dbContext.Courses.SingleOrDefaultAsync(t => t.Id == id);
            if (course == null)
            {
                return NotFound();
            }
            var model = new CreateViewModel
            {
                CourseName = course.Name
            };
            return View(model);
        }
    }
}
