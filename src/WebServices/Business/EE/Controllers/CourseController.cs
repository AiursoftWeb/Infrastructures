using System.Linq;
using System.Threading.Tasks;
using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Attributes;
using Aiursoft.EE.Data;
using Aiursoft.EE.Models;
using Aiursoft.EE.Models.CourseViewModels;
using Aiursoft.AiurProtocol.Models;
using Aiursoft.Identity.Attributes;
using Aiursoft.SDK.Services;
using Aiursoft.CSTools.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.EE.Controllers;


public class CourseController : Controller
{
    private readonly EEDbContext _dbContext;
    private readonly ServiceLocation _serviceLocation;
    private readonly UserManager<EEUser> _userManager;

    public CourseController(
        UserManager<EEUser> userManager,
        EEDbContext dbContext,
        ServiceLocation serviceLocation)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _serviceLocation = serviceLocation;
    }

    [AiurForceAuth]
    public IActionResult Create()
    {
        return View(new CreateViewModel());
    }

    [HttpPost]
    [AiurForceAuth]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateViewModel model)
    {
        var user = await GetCurrentUserAsync();
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var course = new Course
        {
            Description = ScriptsFilter.FilterString(model.Description),
            CourseImage = $"{_serviceLocation.UI}/images/thumbnail.svg",
            DisplayOwnerInfo = model.DisplayOwnerInfo,
            WhatYouWillLearn = model.WhatYouWillLearn,
            Name = model.Name,
            Price = model.Price,
            OwnerId = user.Id
        };
        await _dbContext.Courses.AddAsync(course);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Detail), new { id = course.Id });
    }

    //For users who did not sign in but clicked subscribe
    [AiurForceAuth]
    public IActionResult DetailAuth(int id) // Course id
    {
        return RedirectToAction(nameof(Detail), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id) //Course Id
    {
        var course = await _dbContext
            .Courses
            .Include(t => t.Owner)
            .Include(t => t.Sections)
            .SingleOrDefaultAsync(t => t.Id == id);
        var allChapters = await _dbContext
            .Chapters
            .Include(t => t.Context)
            .Where(t => t.Context.CourseId == course.Id)
            .ToListAsync();
        var user = await GetCurrentUserAsync();
        var subscribed = user != null && await _dbContext
            .Subscriptions
            .SingleOrDefaultAsync(t => t.CourseId == id && t.UserId == user.Id) != null;

        if (course == null)
        {
            return NotFound();
        }

        var model = new DetailViewModel
        {
            Id = id,
            TeacherDescription = course.Owner.LongDescription,
            Name = course.Name,
            Description = course.Description,
            Subscribed = subscribed,
            IsOwner = user?.Id == course.OwnerId,
            AuthorName = course.Owner.NickName,
            DisplayOwnerInfo = course.DisplayOwnerInfo,
            Sections = course.Sections,
            WhatYouWillLearn = course.WhatYouWillLearn,
            Chapters = allChapters
        };
        return View(model);
    }

    [HttpPost]
    [AiurForceAuth("", "", false)]
    [ApiExceptionHandler]
    [ApiModelStateChecker]
    public async Task<IActionResult> Subscribe(int id) //Course Id
    {
        var user = await GetCurrentUserAsync();
        var course = await _dbContext
            .Courses
            .SingleOrDefaultAsync(t => t.Id == id);
        if (course == null)
        {
            return this.Protocol(Code.NotFound, $"The target course with Id:{id} was not found!");
        }

        var subscribed = await _dbContext
            .Subscriptions
            .AnyAsync(t => t.CourseId == id && t.UserId == user.Id);

        if (subscribed)
        {
            return this.Protocol(Code.Conflict, "This course you have already subscribed!");
        }

        var newSubscription = new Subscription
        {
            UserId = user.Id,
            CourseId = id
        };
        await _dbContext.Subscriptions.AddAsync(newSubscription);
        await _dbContext.SaveChangesAsync();
        return this.Protocol(Code.Success, "You have successfully subscribed this course!");
    }

    [HttpPost]
    [AiurForceAuth("", "", false)]
    [ApiExceptionHandler]
    [ApiModelStateChecker]
    public async Task<IActionResult> UnSubscribe(int id) //Course Id
    {
        var user = await GetCurrentUserAsync();
        var course = await _dbContext
            .Courses
            .SingleOrDefaultAsync(t => t.Id == id);
        if (course == null)
        {
            return this.Protocol(Code.NotFound, $"The target course with Id:{id} was not found!");
        }

        var sub = await _dbContext
            .Subscriptions
            .SingleOrDefaultAsync(t => t.CourseId == id && t.UserId == user.Id);

        if (sub == null)
        {
            return this.Protocol(Code.Conflict, "You did not subscribe this course!");
        }

        _dbContext.Subscriptions.Remove(sub);
        await _dbContext.SaveChangesAsync();
        return this.Protocol(Code.Success, "Successfully unsubscribed this course!");
    }

    private async Task<EEUser> GetCurrentUserAsync()
    {
        return await _userManager.GetUserAsync(User);
    }
}