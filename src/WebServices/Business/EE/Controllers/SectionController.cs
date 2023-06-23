using System.Linq;
using System.Threading.Tasks;
using Aiursoft.AiurProtocol;
using Aiursoft.AiurProtocol.Attributes;
using Aiursoft.EE.Data;
using Aiursoft.EE.Models;
using Aiursoft.EE.Models.SectionViewModels;

using Aiursoft.AiurProtocol.Models;
using Aiursoft.Identity.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.EE.Controllers;


public class SectionController : Controller
{
    private readonly EEDbContext _dbContext;
    private readonly UserManager<EEUser> _userManager;

    public SectionController(
        EEDbContext dbContext,
        UserManager<EEUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    [AiurForceAuth]
    // Create new section
    public async Task<IActionResult> Create(int id) // Course Id
    {
        var course = await _dbContext
            .Courses
            .SingleOrDefaultAsync(t => t.Id == id);

        if (course == null)
        {
            return NotFound();
        }

        var user = await GetCurrentUserAsync();
        if (course.OwnerId != user.Id)
        {
            return NotFound();
        }

        var model = new CreateViewModel
        {
            CourseName = course.Name,
            CourseId = course.Id
        };
        return View(model);
    }

    [AiurForceAuth]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateViewModel model) //Course Id
    {
        var course = await _dbContext
            .Courses
            .SingleOrDefaultAsync(t => t.Id == model.CourseId);

        if (course == null)
        {
            return NotFound();
        }

        var user = await GetCurrentUserAsync();
        if (course.OwnerId != user.Id)
        {
            return NotFound();
        }

        var newSection = new Section
        {
            SectionName = model.NewSectionName,
            CourseId = model.CourseId
        };
        await _dbContext.Sections.AddAsync(newSection);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(CourseController.Detail), "Course", new { id = course.Id });
    }

    [AiurForceAuth]
    [HttpPost]
    [ApiExceptionHandler]
    [ApiModelStateChecker]
    public async Task<IActionResult> Drop(int id) //Section Id
    {
        var user = await GetCurrentUserAsync();
        var section = await _dbContext.Sections.Where(t => t.Context.OwnerId == user.Id)
            .SingleOrDefaultAsync(t => t.Id == id);
        if (section == null)
        {
            return NotFound();
        }

        _dbContext.Sections.Remove(section);
        await _dbContext.SaveChangesAsync();
        return this.Protocol(Code.Success, $"Successfully deleted the section '{section.SectionName}'!");
    }

    private async Task<EEUser> GetCurrentUserAsync()
    {
        return await _userManager.GetUserAsync(User);
    }
}