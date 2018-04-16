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

namespace Aiursoft.Pylon.Services
{
    public static class StorageService
    {
        private static async Task<string> _SaveLocally(IFormFile file, SaveFileOptions options = SaveFileOptions.RandomName, string name = "")
        {
            string directoryPath = GetCurrentDirectory() + DirectorySeparatorChar + $@"Storage" + DirectorySeparatorChar;
            if (Exists(directoryPath) == false)
            {
                CreateDirectory(directoryPath);
            }
            string localFilePath = string.Empty;
            if (options == SaveFileOptions.RandomName)
            {
                localFilePath = directoryPath + StringOperation.RandomString(10) + GetExtension(file.FileName);
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
        public static async Task<string> SaveToOSS(IFormFile file, int BucketId, int AliveDays, SaveFileOptions options = SaveFileOptions.RandomName, string AccessToken = null, string name = "", bool deleteLocal = true)
        {
            string localFilePath = await _SaveLocally(file, options, name);
            if (AccessToken == null)
            {
                AccessToken = await AppsContainer.AccessToken()();
            }
            var fileAddress = await ApiService.UploadFile(AccessToken, BucketId, localFilePath, AliveDays);
            if (deleteLocal)
            {
                File.Delete(localFilePath);
            }
            return fileAddress.Path;
        }
    }
    public enum SaveFileOptions
    {
        RandomName,
        SourceName,
        TargetName
    }
}
