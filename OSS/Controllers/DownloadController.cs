using Aiursoft.OSS.Data;
using Aiursoft.OSS.Models.DownloadAddressModels;
using Aiursoft.OSS.Services;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Aiursoft.OSS.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class DownloadController : Controller
    {
        private readonly char _ = Path.DirectorySeparatorChar;
        private readonly OSSDbContext _dbContext;
        private readonly ImageCompressor _imageCompressor;
        private readonly IConfiguration _configuration;
        public DownloadController(
            OSSDbContext dbContext,
            ImageCompressor imageCompressor,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _imageCompressor = imageCompressor;
            _configuration = configuration;
        }

        private async Task<IActionResult> ReturnFile(string path, int h, int w, string realfileName, bool download, string suggestefFileName)
        {
            try
            {
                if (realfileName.IsStaticImage() && h > 0 && w > 0)
                {
                    return await this.AiurFile(await _imageCompressor.Compress(path, realfileName, w, h), realfileName, download, suggestefFileName);
                }
                else
                {
                    return await this.AiurFile(path, realfileName, download, suggestefFileName);
                }
            }
            catch (Exception e) when (e is DirectoryNotFoundException || e is FileNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route(template: "/{BucketName}/{FileName}.{FileExtension}")]
        public async Task<IActionResult> DownloadFile(DownloadFileAddressModel model)
        {
            var download = !string.IsNullOrWhiteSpace(model.SD);
            var targetBucket = await _dbContext.Bucket.SingleOrDefaultAsync(t => t.BucketName == model.BucketName);
            if (targetBucket == null || !targetBucket.OpenToRead)
                return NotFound();
            var targetFile = await _dbContext
                .OSSFile
                .Where(t => t.BucketId == targetBucket.BucketId)
                .SingleOrDefaultAsync(t => t.RealFileName == model.FileName + "." + model.FileExtension);

            if (targetFile == null)
                return NotFound();

            // Update download times
            targetFile.DownloadTimes++;
            await _dbContext.SaveChangesAsync();

            var path = _configuration["StoragePath"] + $"{_}Storage{_}{targetBucket.BucketName}{_}{targetFile.FileKey}.dat";
            return await ReturnFile(path, model.H, model.W, targetFile.RealFileName, download, model.Name);
        }

        [HttpGet]
        public async Task<IActionResult> FromSecret(FromSecretAddressModel model)
        {
            var download = !string.IsNullOrWhiteSpace(model.SD);
            var secret = await _dbContext
                .Secrets
                .Include(t => t.File)
                .SingleOrDefaultAsync(t => t.Value == model.Sec);
            if (secret == null || secret.UsedTimes >= secret.MaxUseTime)
            {
                return NotFound();
            }
            secret.UsedTimes++;
            secret.UseTime = DateTime.UtcNow;
            secret.UserIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            await _dbContext.SaveChangesAsync();
            var bucket = await _dbContext
                .Bucket
                .SingleOrDefaultAsync(t => t.BucketId == secret.File.BucketId);

            var path = _configuration["StoragePath"] + $"{_}Storage{_}{bucket.BucketName}{_}{secret.File.FileKey}.dat";
            return await ReturnFile(path, model.H, model.W, secret.File.RealFileName, download, model.Name);
        }

        [HttpGet]
        [Route(template: "/Download/FromKey/{Id}")]
        [Route(template: "/Download/FromKey/{Id}.{FileExtension=Unset}")]
        public async Task<IActionResult> FromKey(FromKeyAddressModel model)
        {
            var download = !string.IsNullOrWhiteSpace(model.SD);
            var file = await _dbContext
                .OSSFile
                .Include(t => t.BelongingBucket)
                .SingleOrDefaultAsync(t => t.FileKey == model.Id);
            if (file == null)
            {
                return NotFound();
            }
            else if (!file.BelongingBucket.OpenToRead)
            {
                return NotFound();
            }
            file.DownloadTimes++;
            await _dbContext.SaveChangesAsync();
            var path = _configuration["StoragePath"] + $"{_}Storage{_}{file.BelongingBucket.BucketName}{_}{file.FileKey}.dat";
            return await ReturnFile(path, model.H, model.W, file.RealFileName, download, model.Name);
        }
    }
}
