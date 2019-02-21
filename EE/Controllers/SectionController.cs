using System;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.EE.Data;
using Aiursoft.EE.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Aiursoft.Pylon.Attributes;
using Microsoft.EntityFrameworkCore;
using Aiursoft.EE.Models.SectionViewModels;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Models;

namespace Aiursoft.EE.Controllers
{
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
        public async Task<IActionResult> Create(CreateViewModel model)//Course Id
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
            _dbContext.Sections.Add(newSection);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(CourseController.Detail), "Course", new { id = course.Id });
        }

        [AiurForceAuth]
        [HttpPost]
        [APIExpHandler]
        [APIModelStateChecker]
        public async Task<IActionResult> Drop(int id) //Section Id
        {
            var user = await GetCurrentUserAsync();
            var section = await _dbContext.Sections.SingleOrDefaultAsync(t => t.Id == id);
            if (section == null)
            {
                return NotFound();
            }
            _dbContext.Sections.Remove(section);
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, $"Successfully deleted the section '{section.SectionName}'!");
        }

        private async Task<EEUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }
    }
}