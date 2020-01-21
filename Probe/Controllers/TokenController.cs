using Aiursoft.DocGenerator.Attribute;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.Services;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Attributes;
using Aiursoft.SDK.Models.Probe.TokenAddressModels;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class TokenController : Controller
    {
        private readonly ACTokenManager _tokenManager;
        private readonly ProbeDbContext _dbContext;
        private readonly PBTokenManager _pbTokenManager;

        public TokenController(
            ACTokenManager tokenManager,
            ProbeDbContext dbContext,
            PBTokenManager pbTokenManager)
        {
            _tokenManager = tokenManager;
            _dbContext = dbContext;
            _pbTokenManager = pbTokenManager;
        }

        [HttpPost]
        [APIProduces(typeof(AiurValue<string>))]
        public async Task<IActionResult> GetUploadToken(GetUploadTokenAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            var site = await _dbContext
                .Sites
                .SingleOrDefaultAsync(t => t.SiteName == model.SiteName);
            if (site == null)
            {
                return this.Protocol(ErrorType.NotFound, $"Could not find a site with name: '{model.SiteName}'");
            }
            if (site.AppId != appid)
            {
                return this.Protocol(ErrorType.Unauthorized, $"The site '{model.SiteName}' you tried to get a PBToken is not your app's site.");
            }
            var (pbToken, deadline) = _pbTokenManager.GenerateAccessToken(site.SiteName, model.UnderPath, model.Permissions);
            return Json(new AiurValue<string>(pbToken)
            {
                Code = ErrorType.Success,
                Message = $"Successfully get your PBToken! Use it before {deadline} UTC!"
            });
        }
    }
}
