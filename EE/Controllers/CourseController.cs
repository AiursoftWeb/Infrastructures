using Aiursoft.EE.Data;
using Aiursoft.EE.Models;
using Aiursoft.EE.Models.CourseViewModels;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
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
        private readonly UserManager<EEUser> _userManager;
        private readonly SignInManager<EEUser> _signInManager;
        private readonly EEDbContext _dbContext;
        private readonly ServiceLocation _serviceLocation;
        private readonly ScriptsFilter _scriptsFilter;

        public CourseController(
            UserManager<EEUser> userManager,
            SignInManager<EEUser> signInManager,
            EEDbContext dbContext,
            ServiceLocation serviceLocation,
            ScriptsFilter scriptsFilter)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _serviceLocation = serviceLocation;
            _scriptsFilter = scriptsFilter;
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
                Description = _scriptsFilter.Filt(model.Description),
                CourseImage = $"{_serviceLocation.CDN}/images/thumbnail.svg",
                Name = model.Name,
                Price = model.Price,
                OwnerId = user.Id
            };
            _dbContext.Courses.Add(course);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id = course.Id });
        }

        [AiurForceAuth]
        public IActionResult DetailAuth(int id)//Course id
        {
            return RedirectToAction(nameof(Detail), new { id = id });
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
                IsOwner = user?.Id == course.OwnerId,
                AuthorName = course.Owner.NickName,
                DisplayOwnerInfo = course.DisplayOwnerInfo
            };
            return View(model);
        }

        [HttpGet]
        [AiurForceAuth]
        public async Task<IActionResult> Upload(int id)//Course Id
        {
            var course = await _dbContext
                .Courses
                .Include(t => t.Chapters)
                .SingleOrDefaultAsync(t => t.Id == id);
            var user = await GetCurrentUserAsync();
            if (course == null || course.OwnerId != user.Id)
            {
                return NotFound();
            }
            var model = new UploadViewModel();
            return View(model);
        }

        [HttpPost]
        [AiurForceAuth("", "", false)]
        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<IActionResult> Subscribe(int id)//Course Id
        {
            var user = await GetCurrentUserAsync();
            var course = await _dbContext
                .Courses
                .SingleOrDefaultAsync(t => t.Id == id);
            if (course == null)
            {
                return this.Protocal(ErrorType.NotFound, $"The target course with Id:{id} was not found!");
            }

            var sub = await _dbContext
                .Subscriptions
                .SingleOrDefaultAsync(t => t.CourseId == id && user.Id == user.Id);

            if (sub == null)
            {
                var newSubscription = new Subscription
                {
                    UserId = user.Id,
                    CourseId = id
                };
                _dbContext.Subscriptions.Add(newSubscription);
                await _dbContext.SaveChangesAsync();
                return this.Protocal(ErrorType.Success, "You have successfully subscribed this course!");
            }
            return this.Protocal(ErrorType.HasDoneAlready, "This course you have already subscribed!");
        }

        [HttpPost]
        [AiurForceAuth("", "", false)]
        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<IActionResult> UnSubscribe(int id)//Course Id
        {
            var user = await GetCurrentUserAsync();
            var course = await _dbContext
                .Courses
                .SingleOrDefaultAsync(t => t.Id == id);
            if (course == null)
            {
                return this.Protocal(ErrorType.NotFound, $"The target course with Id:{id} was not found!");
            }
            var sub = await _dbContext
                .Subscriptions
                .SingleOrDefaultAsync(t => t.CourseId == id && user.Id == user.Id);

            if (sub != null)
            {
                _dbContext.Subscriptions.Remove(sub);
                await _dbContext.SaveChangesAsync();
                return this.Protocal(ErrorType.Success, "Successfully unsubscribed this course!");
            }
            return this.Protocal(ErrorType.HasDoneAlready, "You did not subscribe this course!");
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
