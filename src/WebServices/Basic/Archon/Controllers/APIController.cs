using Aiursoft.Archon.SDK.Models;
using Aiursoft.Archon.Services;
using Aiursoft.Developer.SDK.Services.ToDeveloperServer;
using Aiursoft.DocGenerator.Attributes;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Aiursoft.Archon.Controllers
{
    [LimitPerMin]
    public class APIController : ControllerBase
    {
        private readonly TokenGenerator _tokenManager;
        private readonly DeveloperApiService _developerApiService;

        public APIController(
            TokenGenerator tokenManager,
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
            var correctApp = await _developerApiService.IsValidAppAsync(model.AppId, model.AppSecret);
            if (correctApp)
            {
                var token = _tokenManager.GenerateAccessToken(model.AppId);
                return this.Protocol(new AccessTokenViewModel
                {
                    Code = ErrorType.Success,
                    Message = "Successfully get access token.",
                    AccessToken = token.tokenString,
                    DeadTime = token.expireTime
                });
            }
            else
            {
                return this.Protocol(ErrorType.WrongKey, "Wrong app info.");
            }
        }
    }
}
