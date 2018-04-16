using Aiursoft.EE.Data;
using Aiursoft.EE.Models;
using Aiursoft.EE.Models.ProfileViewModels;
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
    public class ProfileController : Controller
    {
        public readonly UserManager<EEUser> _userManager;
        public readonly SignInManager<EEUser> _signInManager;
        public readonly EEDbContext _dbContext;

        public ProfileController(
            UserManager<EEUser> userManager,
            SignInManager<EEUser> signInManager,
            EEDbContext _context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = _context;
        }

        public async Task<IActionResult> Overview(string id)
        {
            var user = await _userManager.FindByNameAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var courses = _dbContext.Courses.Where(t => t.OwnerId == user.Id).Take(6);
            var model = new OverviewViewModel
            {
                CoursesDisplaying = courses.Take(6)
            };
            await model.Restore(user, 0, _dbContext, await GetCurrentUserAsync());
            return View(model);
        }

        public async Task<IActionResult> Courses(string id)
        {
            var user = await _userManager.FindByNameAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var courses = _dbContext.Courses.Where(t => t.OwnerId == user.Id);
            var model = new CoursesViewModel
            {
                AllCourses = courses
            };
            await model.Restore(user, 1, _dbContext, await GetCurrentUserAsync());
            return View(model);
        }

        public async Task<IActionResult> Subscriptions(string id)
        {
            var user = await _userManager.FindByNameAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var mySubs = _dbContext
                .Subscriptions
                .Include(t => t.Course)
                .Where(t => t.UserId == user.Id);
            var model = new SubscriptionsViewModel
            {
                MySubscriptions = mySubs
            };
            await model.Restore(user, 2, _dbContext, await GetCurrentUserAsync());
            return View(model);
        }

        [HttpPost]
        [AiurForceAuth]
        public async Task<IActionResult> UnSubscribe(int id) // sub Id
        {
            var user = await GetCurrentUserAsync();
            var sub = await _dbContext.Subscriptions.SingleOrDefaultAsync(t => t.Id == id && t.UserId == user.Id);
            if (sub != null)
            {
                _dbContext.Subscriptions.Remove(sub);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Subscriptions), new { id = user.UserName });
        }

        public async Task<IActionResult> Followings(string id)//Viewing user name
        {
            var user = await _userManager.FindByNameAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var followings = await _dbContext
                .Follows
                .Include(t => t.Receiver)
                .Where(t => t.TriggerId == user.Id).ToListAsync();
            var model = new FollowingsViewModel
            {
                Followings = followings
            };
            await model.Restore(user, 3, _dbContext, await GetCurrentUserAsync());
            return View(model);
        }

        public async Task<IActionResult> Followers(string id)
        {
            var user = await _userManager.FindByNameAsync(id);//Viewing user name
            if (user == null)
            {
                return NotFound();
            }
            var followers = await _dbContext
                .Follows
                .Include(t => t.Trigger)
                .Where(t => t.ReceiverId == user.Id).ToListAsync();
            var model = new FollowersViewModel
            {
                Followers = followers
            };
            await model.Restore(user, 4, _dbContext, await GetCurrentUserAsync());
            return View(model);
        }

        [HttpPost]
        [AiurForceAuth]
        public async Task<IActionResult> Follow(string id)//Target user id
        {
            var cuser = await GetCurrentUserAsync();
            var user = await _userManager.FindByIdAsync(id);
            var follow = await _dbContext.Follows.SingleOrDefaultAsync(t => t.TriggerId == cuser.Id && t.ReceiverId == user.Id);
            if (follow == null)
            {
                _dbContext.Follows.Add(new Follow
                {
                    TriggerId = cuser.Id,
                    ReceiverId = user.Id
                });
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Overview), new { id = user.Id });
        }

        [HttpPost]
        [AiurForceAuth]
        public async Task<IActionResult> UnFollow(string id, string redirectAction)//target user id
        {
            var cuser = await GetCurrentUserAsync();
            var user = await _userManager.FindByIdAsync(id);
            var follow = await _dbContext.Follows.SingleOrDefaultAsync(t => t.TriggerId == cuser.Id && t.ReceiverId == user.Id);
            if (follow != null)
            {
                _dbContext.Follows.Remove(follow);
                await _dbContext.SaveChangesAsync();
            }
            if (redirectAction == nameof(Overview))
            {
                return RedirectToAction(redirectAction, new { id = user.Id });
            }
            else if(redirectAction == nameof(Followings))
            {
                return RedirectToAction(redirectAction, new { id = cuser.Id });
            }
            throw new InvalidOperationException();
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
