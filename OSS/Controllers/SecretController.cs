using Aiursoft.OSS.Data;
using Aiursoft.Pylon;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models.OSS;
using Aiursoft.Pylon.Models.OSS.SecretAddressModels;

namespace Aiursoft.OSS.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class SecretController : Controller
    {
        private readonly OSSDbContext _dbContext;
        private readonly ACTokenManager _tokenManager;

        public SecretController(
            OSSDbContext dbContext,
            ACTokenManager tokenManager)
        {
            _dbContext = dbContext;
            _tokenManager = tokenManager;
        }

        [HttpGet]
        public async Task<IActionResult> Generate(GenerateAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            var file = await _dbContext
                .OSSFile
                .Include(t => t.BelongingBucket)
                .Where(t => t.BelongingBucket.BelongingAppId == appid)
                .SingleOrDefaultAsync(t => t.FileKey == model.Id);
            if (file == null)
            {
                return this.Protocal(ErrorType.NotFound, "Could not get your file in your apps' buckets. The file may be out dated!");
            }
            // Generate secret
            var newSecret = new Secret
            {
                Value = Guid.NewGuid().ToString("N"),
                FileId = file.FileKey,
                MaxUseTime = model.MaxUseTimes
            };
            _dbContext.Secrets.Add(newSecret);
            await _dbContext.SaveChangesAsync();
            return Json(new AiurValue<string>(newSecret.Value)
            {
                Code = ErrorType.Success,
                Message = "Successfully created your onetime secret!"
            });
        }
    }
}
