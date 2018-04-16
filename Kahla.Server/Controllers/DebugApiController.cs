using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kahla.Server.Data;
using Kahla.Server.Models;
using Microsoft.AspNetCore.Identity;
using Aiursoft.Pylon.Attributes;
using Kahla.Server.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Kahla.Server.Services;
using Aiursoft.Pylon.Services;

namespace Kahla.Server.Controllers
{
    public class DebugApiController : ApiController
    {
        public DebugApiController(
            UserManager<KahlaUser> userManager,
            SignInManager<KahlaUser> signInManager,
            KahlaDbContext dbContext,
            PushKahlaMessageService pushService,
            IConfiguration configuration,
            AuthService<KahlaUser> authService) : base(userManager, signInManager, dbContext, pushService, configuration, authService)
        {
        }
    }
}
