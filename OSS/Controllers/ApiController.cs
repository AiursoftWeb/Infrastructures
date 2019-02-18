using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToAPIServer;
using Aiursoft.OSS.Data;
using Aiursoft.OSS.Models;
using Aiursoft.Pylon.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models.OSS.ApiViewModels;
using Aiursoft.Pylon.Models.OSS;
using Aiursoft.Pylon.Models.API.ApiViewModels;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models.OSS.ApiAddressModels;
using Aiursoft.Pylon;
using Aiursoft.OSS.Services;
using Microsoft.Extensions.Configuration;
using Aiursoft.Pylon.Services.ToDeveloperServer;

namespace Aiursoft.OSS.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    public class ApiController : Controller
    {
        private readonly char _ = Path.DirectorySeparatorChar;
        private readonly OSSDbContext _dbContext;
        private readonly ImageCompresser _imageCompresser;
        private readonly IConfiguration _configuration;
        private readonly ServiceLocation _serviceLocation;
        private readonly CoreApiService _coreApiService;
        private readonly ACTokenManager _tokenManager;
        private readonly object _obj = new object();

        public ApiController(
            OSSDbContext dbContext,
            ImageCompresser imageCompresser,
            IConfiguration configuration,
            ServiceLocation serviceLocation,
            CoreApiService coreApiService,
            ACTokenManager tokenManager)
        {
            _dbContext = dbContext;
            _imageCompresser = imageCompresser;
            _configuration = configuration;
            _serviceLocation = serviceLocation;
            _coreApiService = coreApiService;
            _tokenManager = tokenManager;
        }

        [HttpPost]
        public async Task<JsonResult> DeleteApp(DeleteAppAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            if (appid != model.AppId)
            {
                return this.Protocal(ErrorType.Unauthorized, "The app you try to delete is not the accesstoken you granted!");
            }
            var target = await _dbContext.Apps.FindAsync(appid);
            if (target != null)
            {
                _dbContext.OSSFile.RemoveRange(_dbContext.OSSFile.Include(t => t.BelongingBucket).Where(t => t.BelongingBucket.BelongingAppId == target.AppId));
                _dbContext.Bucket.Delete(t => t.BelongingAppId == target.AppId);
                _dbContext.Apps.Remove(target);
                await _dbContext.SaveChangesAsync();
                return this.Protocal(ErrorType.Success, "Successfully deleted that app and all files.");
            }
            return this.Protocal(ErrorType.HasDoneAlready, "That app do not exists in our database.");
        }

        public async Task<JsonResult> ViewMyBuckets(ViewMyBucketsAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            var appLocal = await _dbContext.Apps.SingleOrDefaultAsync(t => t.AppId == appid);
            if (appLocal == null)
            {
                appLocal = new OSSApp
                {
                    AppId = appid,
                    MyBuckets = new List<Bucket>()
                };
                _dbContext.Apps.Add(appLocal);
                await _dbContext.SaveChangesAsync();
            }

            var buckets = await _dbContext
                .Bucket
                .Include(t => t.Files)
                .Where(t => t.BelongingAppId == appid)
                .ToListAsync();
            buckets.ForEach(t => t.FileCount = t.Files.Count());
            var viewModel = new ViewMyBucketsViewModel
            {
                AppId = appLocal.AppId,
                Buckets = buckets,
                Code = ErrorType.Success,
                Message = "Successfully get your buckets!"
            };
            return Json(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> CreateBucket([FromForm]CreateBucketAddressModel model)
        {
            //Update app info
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            var appLocal = await _dbContext.Apps.Include(t => t.MyBuckets).SingleOrDefaultAsync(t => t.AppId == appid);
            if (appLocal == null)
            {
                appLocal = new OSSApp
                {
                    AppId = appid,
                    MyBuckets = new List<Bucket>()
                };
                _dbContext.Apps.Add(appLocal);
            }
            //Ensure not exists
            var existing = await _dbContext.Bucket.SingleOrDefaultAsync(t => t.BucketName == model.BucketName);
            if (existing != null)
            {
                return this.Protocal(ErrorType.NotEnoughResources, "There is one bucket already called that name!");
            }
            //Create and save to database
            var newBucket = new Bucket
            {
                BucketName = model.BucketName,
                Files = new List<OSSFile>(),
                OpenToRead = model.OpenToRead,
                OpenToUpload = model.OpenToUpload
            };
            appLocal.MyBuckets.Add(newBucket);
            await _dbContext.SaveChangesAsync();
            //Create an empty folder
            string DirectoryPath = _configuration["StoragePath"] + $@"{_}Storage{_}{newBucket.BucketName}{_}";
            if (Directory.Exists(DirectoryPath) == false)
            {
                Directory.CreateDirectory(DirectoryPath);
            }
            //return model
            var viewModel = new CreateBucketViewModel
            {
                BucketId = newBucket.BucketId,
                Code = ErrorType.Success,
                Message = "Successfully created your bucket!"
            };
            return Json(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> EditBucket([FromForm]EditBucketAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            var existing = _dbContext.Bucket.Any(t => t.BucketName == model.NewBucketName && t.BucketId != model.BucketId);
            if (existing)
            {
                return this.Protocal(ErrorType.NotEnoughResources, "There is one bucket already called that name!");
            }
            var target = await _dbContext.Bucket.FindAsync(model.BucketId);
            if (target == null)
            {
                return this.Protocal(ErrorType.NotFound, "Not found target bucket!");
            }
            else if (target.BelongingAppId != appid)
            {
                return this.Protocal(ErrorType.Unauthorized, "This is not your bucket!");
            }
            var oldpath = _configuration["StoragePath"] + $@"{_}Storage{_}{target.BucketName}";
            var newpath = _configuration["StoragePath"] + $@"{_}Storage{_}{model.NewBucketName}";
            if (oldpath != newpath)
            {
                new DirectoryInfo(oldpath).MoveTo(newpath);
            }
            target.BucketName = model.NewBucketName;
            target.OpenToRead = model.OpenToRead;
            target.OpenToUpload = model.OpenToUpload;
            await _dbContext.SaveChangesAsync();
            return this.Protocal(ErrorType.Success, "Successfully edited your bucket!");
        }

        public async Task<JsonResult> ViewBucketDetail(ViewBucketDetailAddressModel model)
        {
            var targetBucket = await _dbContext.Bucket.FindAsync(model.BucketId);
            if (targetBucket == null)
            {
                return this.Protocal(ErrorType.NotFound, "Can not find target bucket!");
            }
            var viewModel = new ViewBucketViewModel(targetBucket)
            {
                Code = ErrorType.Success,
                Message = "Successfully get your bucket info!",
                FileCount = await _dbContext.OSSFile.Where(t => t.BucketId == targetBucket.BucketId).CountAsync()
            };
            return Json(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> DeleteBucket([FromForm]DeleteBucketAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            var bucket = await _dbContext.Bucket.FindAsync(model.BucketId);
            if (bucket.BelongingAppId != appid)
            {
                return this.Protocal(ErrorType.Unauthorized, "The bucket you try to delete is not your app's bucket!");
            }
            _dbContext.Bucket.Remove(bucket);
            _dbContext.OSSFile.RemoveRange(_dbContext.OSSFile.Where(t => t.BucketId == bucket.BucketId));
            await _dbContext.SaveChangesAsync();
            return this.Protocal(ErrorType.Success, "Successfully deleted your bucket!");
        }

        public async Task<JsonResult> ViewOneFile(ViewOneFileAddressModel model)
        {
            var file = await _dbContext
                .OSSFile
                .Include(t => t.BelongingBucket)
                .SingleOrDefaultAsync(t => t.FileKey == model.FileKey);
            if (file == null || file.BelongingBucket == null)
            {
                return this.Protocal(ErrorType.NotFound, "Could not find a valid file in OSS!");
            }
            var path = _configuration["StoragePath"] + $@"{_}Storage{_}{file.BelongingBucket.BucketName}{_}{file.FileKey}.dat";
            file.JFileSize = new FileInfo(path).Length;
            file.InternetPath = new AiurUrl(_serviceLocation.OSS, file.BelongingBucket.BucketName, file.RealFileName, new { }).ToString();

            var viewModel = new ViewOneFileViewModel
            {
                File = file,
                Code = ErrorType.Success,
                Message = "Successfully get that file!"
            };
            return Json(viewModel);
        }

        public async Task<JsonResult> ViewMultiFiles(ViewMultiFilesAddressModel model)
        {
            var ids = model.Ids.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var list = new List<OSSFile>();
            foreach(var id in ids)
            {
                var fileKey = Convert.ToInt32(id);
                var file = await _dbContext
                    .OSSFile
                    .Include(t => t.BelongingBucket)
                    .SingleOrDefaultAsync(t => t.FileKey == fileKey);
                var path = _configuration["StoragePath"] + $@"{_}Storage{_}{file.BelongingBucket.BucketName}{_}{file.FileKey}.dat";
                file.JFileSize = new FileInfo(path).Length;
                file.InternetPath = new AiurUrl(_serviceLocation.OSS, file.BelongingBucket.BucketName, file.RealFileName, new { }).ToString();
                list.Add(file);
            }
            return Json(new AiurCollection<OSSFile>(list)
            {
                Code = ErrorType.Success,
                Message = "Successfully get all files you queried."
            });
        }

        [HttpPost]
        [FileChecker]
        public async Task<JsonResult> UploadFile(UploadFileAddressModel model)
        {
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            //try find the target bucket
            var targetBucket = await _dbContext.Bucket.FindAsync(model.BucketId);
            if (targetBucket == null || targetBucket.BelongingAppId != appid)
            {
                return this.Protocal(ErrorType.Unauthorized, "The bucket you try to upload is not your app's bucket!");
            }
            //try get the file from form
            var file = Request.Form.Files.First();
            var newFile = new OSSFile
            {
                RealFileName = Path.GetFileName(file.FileName.Replace(" ", "")),
                FileExtension = Path.GetExtension(file.FileName),
                BucketId = targetBucket.BucketId,
                AliveDays = model.AliveDays,
                UploadTime = DateTime.UtcNow
            };
            //Ensure there not exists file with the same file name.
            lock (_obj)
            {
                var exists = _dbContext.OSSFile.Any(t => t.RealFileName == newFile.RealFileName && t.BucketId == newFile.BucketId);
                if (exists)
                {
                    return this.Protocal(ErrorType.HasDoneAlready, "There already exists a file with that name.");
                }
                //Save to database
                _dbContext.OSSFile.Add(newFile);
                _dbContext.SaveChanges();
            }
            //Try saving file.
            string DirectoryPath = _configuration["StoragePath"] + $"{_}Storage{_}{targetBucket.BucketName}{_}";
            if (Directory.Exists(DirectoryPath) == false)
            {
                Directory.CreateDirectory(DirectoryPath);
            }
            using (var fileStream = new FileStream(DirectoryPath + newFile.FileKey + ".dat", FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
                fileStream.Close();
            }
            // Get Internet path
            newFile.InternetPath = new AiurUrl(_serviceLocation.OSS, newFile.BelongingBucket.BucketName, newFile.RealFileName, new { }).ToString();
            //Return json
            return Json(new UploadFileViewModel
            {
                Code = ErrorType.Success,
                FileKey = newFile.FileKey,
                Message = "Successfully uploaded your file.",
                Path = newFile.InternetPath
            });
        }

        public async Task<JsonResult> ViewAllFiles(CommonAddressModel model)
        {
            //Analyse app
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            var bucket = await _dbContext.Bucket.FindAsync(model.BucketId);
            //Security
            if (bucket.BelongingAppId != appid)
            {
                return this.Protocal(ErrorType.Unauthorized, "The bucket you tried to view is not that app's bucket.");
            }
            //Get all files.
            var allFiles = _dbContext.OSSFile.Include(t => t.BelongingBucket).Where(t => t.BucketId == bucket.BucketId).Take(200);
            foreach (var file in allFiles)
            {
                var path = _configuration["StoragePath"] + $@"{_}Storage{_}{file.BelongingBucket.BucketName}{_}{file.FileKey}.dat";
                file.JFileSize = new FileInfo(path).Length;
                file.InternetPath = new AiurUrl(_serviceLocation.OSS, file.BelongingBucket.BucketName, file.RealFileName, new { }).ToString();
            }
            var viewModel = new ViewAllFilesViewModel
            {
                AllFiles = allFiles,
                BucketId = bucket.BucketId,
                Message = "Successfully get all your files of that bucket.",
                Code = ErrorType.Success
            };
            return Json(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> DeleteFile(DeleteFileAddressModel model)
        {
            //Analyse app
            var appid = _tokenManager.ValidateAccessToken(model.AccessToken);
            var file = await _dbContext
                .OSSFile
                .Include(t => t.BelongingBucket)
                .SingleOrDefaultAsync(t => t.FileKey == model.FileKey);
            if (file == null || file.BelongingBucket == null)
            {
                return this.Protocal(ErrorType.NotFound, "We did not find that file in that bucket!");
            }
            //Security
            if (file.BelongingBucket.BelongingAppId != appid)
            {
                return this.Protocal(ErrorType.Unauthorized, "The bucket you tried is not that app's bucket.");
            }
            //Delete file in disk
            var path = _configuration["StoragePath"] + $@"{_}Storage{_}{file.BelongingBucket.BucketName}{_}{file.FileKey}.dat";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            //Delete file in database
            _dbContext.OSSFile.Remove(file);
            await _dbContext.SaveChangesAsync();
            return this.Protocal(ErrorType.Success, "Successfully deleted your file!");
        }
    }
}