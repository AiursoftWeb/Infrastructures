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
using Aiursoft.Pylon.Services.ToProbeServer;

namespace Aiursoft.Pylon.Services
{
    public class StorageService
    {
        private readonly OSSApiService _ossApiService;
        private readonly AppsContainer _appsContainer;
        private readonly FilesService _filesService;
        private readonly ServiceLocation _serviceLocation;

        public StorageService(
            OSSApiService ossApiService,
            AppsContainer appsContainer,
            FilesService filesService,
            ServiceLocation serviceLocation)
        {
            _ossApiService = ossApiService;
            _appsContainer = appsContainer;
            _filesService = filesService;
            _serviceLocation = serviceLocation;
        }

        [Obsolete]
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

        [Obsolete]
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

        public async Task<Models.Probe.FilesViewModels.UploadFileViewModel> SaveToProbe(IFormFile file, string siteName, string path, SaveFileOptions options = SaveFileOptions.RandomName, string accessToken = null)
        {
            string fileName = options == SaveFileOptions.RandomName ?
                Guid.NewGuid().ToString("N") + GetExtension(file.FileName) :
                file.FileName;
            if (accessToken == null)
            {
                accessToken = await _appsContainer.AccessToken();
            }
            var result = await _filesService.UploadFileAsync(accessToken, siteName, path, file.OpenReadStream(), fileName, true);
            return result;
        }

        public static string GetProbeDownloadAddress(ServiceLocation serviceLocation, string siteName, string path, string fileName)
        {
            var filePath = $"{path}/{fileName}".TrimStart('/');
            return $"{serviceLocation.Probe}/Download/InSites/{siteName}/{filePath}";
        }

        public static string GetProbeDownloadAddress(ServiceLocation serviceLocation, string fullpath)
        {
            return $"{serviceLocation.Probe}/Download/InSites/{fullpath}";
        }
    }

    public enum SaveFileOptions
    {
        RandomName,
        SourceName
    }
}
