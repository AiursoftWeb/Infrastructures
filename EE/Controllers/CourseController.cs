using Aiursoft.EE.Data;
using Aiursoft.EE.Models;
using Aiursoft.EE.Models.CourseViewModels;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.EE.Controllers
{
    public class CourseController : Controller
    {
        public readonly UserManager<EEUser> _userManager;
        public readonly SignInManager<EEUser> _signInManager;
        public readonly EEDbContext _dbContext;

        public CourseController(
            UserManager<EEUser> userManager,
            SignInManager<EEUser> signInManager,
            EEDbContext _context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = _context;
        }

        [AiurForceAuth]
        public IActionResult Create()
        {
            return View(new CreateViewModel());
        }

        [HttpPost]
        [AiurForceAuth]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            var user = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                model.ModelStateValid = false;
                return View(model);
            }
            var course = new Course
            {
                Description = model.Description,
                Name = model.Name,
                Price = model.Price,
                OwnerId = user.Id
            };
            _dbContext.Courses.Add(course);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id = course.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)//Course Id
        {
            var course = await _dbContext
                .Courses
                .Include(t => t.Owner)
                .SingleOrDefaultAsync(t => t.Id == id);
            var user = await GetCurrentUserAsync();
            var Subscribed = user == null ? false : await _dbContext
                .Subscriptions
                .SingleOrDefaultAsync(t => t.CourseId == id && t.UserId == user.Id) != null;

            if (course == null)
            {
                return NotFound();
            }
            var model = new DetailViewModel
            {
                Id = id,
                Name = course.Name,
                Description = course.Description,
                Subscribed = Subscribed,
                IsOwner = user.Id == course.OwnerId,
                AuthorName = course.Owner.NickName,
                DisplayOwnerInfo = course.DisplayOwnerInfo
            };
            return View(model);
        }

        [HttpPost]
        [AiurForceAuth("", "", false)]
        public async Task<IActionResult> Subscribe(int id)//Course Id
        {
            var user = await GetCurrentUserAsync();
            var sub = await _dbContext
                .Subscriptions
                .SingleOrDefaultAsync(t => t.CourseId == id && t.UserId == user.Id);

            if (sub == null)
            {
                var newSub = new Subscription
                {
                    UserId = user.Id,
                    CourseId = id
                };
                _dbContext.Subscriptions.Add(newSub);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Detail), new { id = id });
        }

        [HttpPost]
        [AiurForceAuth]
        public async Task<IActionResult> UnSubscribe(int id) // course Id
        {
            var user = await GetCurrentUserAsync();
            var sub = await _dbContext.Subscriptions.SingleOrDefaultAsync(t => t.UserId == user.Id && t.CourseId == id);
            if (sub != null)
            {
                _dbContext.Subscriptions.Remove(sub);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Detail), new { id = id });
        }

        private async Task<EEUser> GetCurrentUserAsync()
        {
            if (User.Identity.Name == null)
            {
                return null;
            }
            return await _userManager.FindByNameAsync(User.Identity.Name);
        }
    }
}
