using Aiursoft.Archon.SDK.Models;
using Aiursoft.Archon.Services;
using Aiursoft.Developer.SDK.Services.ToDeveloperServer;
using Aiursoft.DocGenerator.Attributes;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.WebTools;
using Aiursoft.XelNaga.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Aiursoft.Archon.Controllers
{
    [LimitPerMin]
    public class APIController : ControllerBase
    {
        private readonly TokenGenerator _tokenManager;
        private readonly DeveloperApiService _developerApiService;
        private readonly AiurCache _cache;

        public APIController(
            TokenGenerator tokenManager,
            DeveloperApiService developerApiService,
            AiurCache cache)
        {
            _tokenManager = tokenManager;
            _developerApiService = developerApiService;
            _cache = cache;
        }

        [APIExpHandler]
        [APIModelStateChecker]
        [APIProduces(typeof(AccessTokenViewModel))]
        public async Task<IActionResult> AccessToken(AccessTokenAddressModel model)
        {
            var cacheKey = $"Id-{model.AppId}-Secret-{model.AppSecret}";
            var correctApp = await _cache.GetAndCache(cacheKey, () => _developerApiService.IsValidAppAsync(model.AppId, model.AppSecret));
            if (correctApp)
            {
                var token = _tokenManager.GenerateAccessToken(model.AppId);
                return Ok(new AccessTokenViewModel
                {
                    Code = ErrorType.Success,
                    Message = "Successfully get access token.",
                    AccessToken = token.Item1,
                    DeadTime = token.Item2
                });
            }
            else
            {
                return this.Protocol(ErrorType.WrongKey, "Wrong app info.");
            }
        }
    }
}
