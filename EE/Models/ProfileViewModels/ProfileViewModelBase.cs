using Aiursoft.EE.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.EE.Models.ProfileViewModels
{
    public class ProfileViewModelBase
    {
        public SharedLeftContent LeftContent { get; set; }
        public SharedUpContent UpContent { get; set; }
        public async Task Restore(EEUser user/*Target user*/, int panel, EEDbContext dbContext, EEUser currentUser)
        {
            LeftContent = new SharedLeftContent
            {
                UserNickName = user.NickName,
                UserName = user.UserName,
                Email = user.Email,
                UserIconFilePath = user.IconFilePath,
                UserId = user.Id,
                AlreadyFollowed = currentUser != null ? await dbContext
                    .Follows
                    .Where(t => t.TriggerId == currentUser.Id && t.ReceiverId == user.Id)
                    .CountAsync() > 0 : false,
                IsMe = user.Id == currentUser?.Id,
                Bio = user.Bio
            };
            UpContent = new SharedUpContent
            {
                ActivePanel = panel,
                UserName = user.UserName,
                SubScribeCount = await dbContext
                    .Subscriptions
                    .Where(t => t.UserId == user.Id)
                    .CountAsync(),
                FollowerCount = await dbContext
                    .Follows
                    .Where(t => t.ReceiverId == user.Id)
                    .CountAsync(),
                FollowingCount = await dbContext
                    .Follows
                    .Where(t => t.TriggerId == user.Id)
                    .CountAsync(),
                CoursesCount = await dbContext
                    .Courses
                     .Where(t => t.OwnerId == user.Id)
                     .CountAsync()
            };
        }
    }

    public class SharedLeftContent
    {
        public string UserNickName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string UserIconFilePath { get; set; }
        public string UserId { get; set; }
        public bool AlreadyFollowed { get; set; }
        public bool IsMe { get; set; }
        public string Bio { get; set; }
    }
    public class SharedUpContent
    {
        public string UserName { get; set; }
        public int ActivePanel { get; set; } = -1;
        public int SubScribeCount { get; set; } = -1;
        public int FollowingCount { get; set; } = -1;
        public int FollowerCount { get; set; } = -1;
        public int CoursesCount { get; set; } = -1;
    }
}
