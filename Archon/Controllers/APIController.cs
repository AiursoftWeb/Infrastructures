using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Archon;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToDeveloperServer;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Aiursoft.Archon.Controllers
{
    [LimitPerMin]
    public class APIController : Controller
    {
        private readonly ACTokenManager _tokenManager;
        private readonly DeveloperApiService _developerApiService;

        public APIController(
            ACTokenManager tokenManager,
            DeveloperApiService developerApiService)
        {
            _tokenManager = tokenManager;
            _developerApiService = developerApiService;
        }

        [APIExpHandler]
        [APIModelStateChecker]
        [APIProduces(typeof(AccessTokenViewModel))]
        public async Task<IActionResult> AccessToken(AccessTokenAddressModel model)
        {
            await _developerApiService.IsValidAppAsync(model.AppId, model.AppSecret);
            var token = _tokenManager.GenerateAccessToken(model.AppId);
            return Json(new AccessTokenViewModel
            {
                Code = ErrorType.Success,
                Message = "Successfully get access token.",
                AccessToken = token.Item1,
                DeadTime = token.Item2
            });
        }
    }
}
