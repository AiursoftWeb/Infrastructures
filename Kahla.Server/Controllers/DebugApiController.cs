// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Kahla.Server.Data;
// using Kahla.Server.Models;
// using Microsoft.AspNetCore.Identity;
// using Aiursoft.Pylon.Attributes;
// using Kahla.Server.Attributes;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Configuration;
// using Kahla.Server.Services;
// using Aiursoft.Pylon.Services;
// using Aiursoft.Pylon;
// using Aiursoft.Pylon.Services.ToAPIServer;
// using Aiursoft.Pylon.Services.ToStargateServer;
// using Aiursoft.Pylon.Models;

// namespace Kahla.Server.Controllers
// {
//     public class DebugApiController : ApiController
//     {
//         public DebugApiController(
//             UserManager<KahlaUser> userManager,
//             SignInManager<KahlaUser> signInManager,
//             KahlaDbContext dbContext,
//             PushKahlaMessageService pushService,
//             IConfiguration configuration,
//             AuthService<KahlaUser> authService,
//             ServiceLocation serviceLocation,
//             OAuthService oauthService,
//             ChannelService channelService,
//             StorageService storageService,
//             AppsContainer appsContainer,
//             UserService userService) : base(
//                 userManager,
//                 signInManager,
//                 dbContext,
//                 pushService,
//                 configuration,
//                 authService,
//                 serviceLocation,
//                 oauthService,
//                 channelService,
//                 storageService,
//                 appsContainer,
//                 userService)
//         {
//         }
//     }
// }
