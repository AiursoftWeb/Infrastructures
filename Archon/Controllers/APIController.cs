using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.Archon;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToDeveloperServer;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Archon.Controllers
{
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
        public async Task<IActionResult> AccessToken(AccessTokenAddressModel model)
        {
            try
            {
                await _developerApiService.IsValidAppAsync(model.AppId, model.AppSecret);
            }
            catch (AiurUnexceptedResponse e)
            {
                return Json(e.Response);
            }
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
