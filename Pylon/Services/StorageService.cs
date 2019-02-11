using System;
using System.Collections.Generic;
using System.Text;
using static System.IO.Path;
using static System.IO.Directory;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Aiursoft.Pylon.Services.ToOSSServer;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Models.OSS.ApiViewModels;

namespace Aiursoft.Pylon.Services
{
    public class StorageService
    {
        private readonly OSSApiService _ossApiService;
        private readonly AppsContainer _appsContainer;
        public StorageService(
            OSSApiService ossApiService,
            AppsContainer appsContainer)
        {
            _ossApiService = ossApiService;
            _appsContainer = appsContainer;
        }
        private async Task<string> _SaveLocally(IFormFile file, SaveFileOptions options = SaveFileOptions.RandomName, string name = "")
        {
            string directoryPath = GetCurrentDirectory() + DirectorySeparatorChar + $@"Storage" + DirectorySeparatorChar;
            if (Exists(directoryPath) == false)
            {
                CreateDirectory(directoryPath);
            }
            string localFilePath = string.Empty;
            if (options == SaveFileOptions.RandomName)
            {
                localFilePath = directoryPath + Guid.NewGuid().ToString("N") + GetExtension(file.FileName);
            }
            else if (options == SaveFileOptions.SourceName)
            {
                localFilePath = directoryPath + file.FileName.Replace(" ", "_");
            }
            else
            {
                localFilePath = directoryPath + name;
            }
            var fileStream = new FileStream(localFilePath, FileMode.Create);
            await file.CopyToAsync(fileStream);
            fileStream.Close();
            return localFilePath;
        }

        public async Task<UploadFileViewModel> SaveToOSS(IFormFile file, int bucketId, int aliveDays, SaveFileOptions options = SaveFileOptions.RandomName, string accessToken = null, string name = "", bool deleteLocal = true)
        {
            string localFilePath = await _SaveLocally(file, options, name);
            if (accessToken == null)
            {
                accessToken = await _appsContainer.AccessToken();
            }
            var fileAddress = await _ossApiService.UploadFile(accessToken, bucketId, localFilePath, aliveDays);
            if (deleteLocal)
            {
                File.Delete(localFilePath);
            }
            return fileAddress;
        }
    }
    public enum SaveFileOptions
    {
        RandomName,
        SourceName,
        TargetName
    }
}
