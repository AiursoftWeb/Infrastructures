using Aiursoft.OSS.Data;
using Aiursoft.OSS.Models;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Services.ToAPIServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
        private readonly CoreApiService _coreApiService;
        public SecretController(
            OSSDbContext dbContext,
            CoreApiService coreApiService)
        {
            _dbContext = dbContext;
            _coreApiService = coreApiService;
        }

        [HttpGet]
        public async Task<IActionResult> Generate(GenerateAddressModel model)
        {
            var app = await _coreApiService.ValidateAccessTokenAsync(model.AccessToken);
            var file = await _dbContext
                .OSSFile
                .Include(t => t.BelongingBucket)
                .Where(t => t.BelongingBucket.BelongingAppId == app.AppId)
                .SingleOrDefaultAsync(t => t.FileKey == model.Id);
            if (file == null)
            {
                return this.Protocal(ErrorType.NotFound, "Could not get your file in your apps' buckets. The file may be out dated!");
            }
            // Generate secret
            var newSecret = new Secret
            {
                Value = Guid.NewGuid().ToString("N"),
                FileId = file.FileKey
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
